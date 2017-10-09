using Http2;
using Http2.Hpack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static Http2.IoStreamExtensions;

namespace AVS.Core.Http2
{
    internal class Http2Client : IDisposable
    {
        private Connection _connection;
        private Uri _uri;
        private bool _isSsl;

        public Http2Client(Uri uri)
        {
            _uri = uri;
            _isSsl = uri.Scheme == "https";
            Headers = new List<HeaderField>();
        }

        #region Properties
        public ICollection<HeaderField> Headers { get; }
        #endregion

        public async void ConnectAsync()
        {
            if (_connection != null)
            {
                _connection.CloseNow().Wait();
            }

            ConnectionConfiguration connectionConfig = new ConnectionConfigurationBuilder(false)
                .UseSettings(Settings.Default)
                .UseHuffmanStrategy(HuffmanStrategy.IfSmaller)
                .Build();

            TcpClient tcpClient = new TcpClient();

            await tcpClient.ConnectAsync(_uri.Host, _uri.Port);
            tcpClient.NoDelay = true;

            if (_isSsl)
            {
                SslStream sslStream = new SslStream(tcpClient.GetStream());
                await sslStream.AuthenticateAsClientAsync(_uri.Host);

                var wrappedStreams = sslStream.CreateStreams();
                _connection = new Connection(connectionConfig, wrappedStreams.ReadableStream, wrappedStreams.WriteableStream);
            }
            else
            {
                var wrappedStreams = tcpClient.Client.CreateStreams();
                _connection = new Connection(connectionConfig, wrappedStreams.ReadableStream, wrappedStreams.WriteableStream);
            }
        }

        public async Task<Http2ResponseMessage> GetAsync(string path)
        {
            IStream stream = await MakeRequest("GET", path);

            return await GetResponse(stream);
        }

        public async Task<Http2ResponseMessage> PostAsync(string path, HttpContent content)
        {
            content.Headers.Select(header => new HeaderField() { Name = header.Key, Value = string.Join(";", header.Value) });

            IStream stream = await MakeRequest("POST", path);

            var contentBytes = await content.ReadAsByteArrayAsync();
            var buffer = new ArraySegment<byte>(contentBytes);
            await stream.WriteAsync(buffer);

            return await GetResponse(stream);
        }

        private async Task<IStream> MakeRequest(string method, string path)
        {
            HeaderField[] basicHeaders = new HeaderField[]{
                new HeaderField(){Name = ":method", Value = method},
                new HeaderField(){Name = ":scheme", Value = _uri.Scheme},
                new HeaderField(){Name = ":path", Value = path},
                new HeaderField(){Name = ":authority", Value = $"{_uri.Host}:{_uri.Port}"},
            };

            var headers = basicHeaders.Concat(Headers).Distinct(new HeaderFieldComparer());

            IStream stream = await _connection.CreateStreamAsync(headers);

            return stream;
        }

        private async Task<Http2ResponseMessage> GetResponse(IStream stream)
        {
            IEnumerable<HeaderField> responseHeaders = await stream.ReadHeadersAsync();

            List<byte> responseData = new List<byte>();
            byte[] buf = new byte[8192];
            while (true)
            {
                StreamReadResult res = await stream.ReadAsync(new ArraySegment<byte>(buf));
                if (res.EndOfStream) break;
                responseData.AddRange(buf.Take(res.BytesRead));
            }

            await stream.CloseAsync();

            return new Http2ResponseMessage(responseHeaders, responseData.ToArray());
        }

        #region IDisposable implementation
        public void Dispose()
        {
            _connection.CloseNow().Wait();
        }
        #endregion
    }

    internal class HeaderFieldComparer : IEqualityComparer<HeaderField>
    {
        public bool Equals(HeaderField x, HeaderField y)
        {
            return x.Name.Equals(y.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(HeaderField obj)
        {
            int hash = 13;
            hash = (hash * 7) + obj.Name.GetHashCode();
            hash = (hash * 7) + obj.Value.GetHashCode();
            hash = (hash * 7) + obj.Sensitive.GetHashCode();

            return hash;
        }
    }
}
