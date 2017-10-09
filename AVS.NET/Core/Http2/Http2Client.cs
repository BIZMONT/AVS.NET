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
            Headers = new Dictionary<string, string>();
        }

        #region Properties
        public IDictionary<string, string> Headers { get; private set; }
        #endregion

        #region Public methods
        public async Task ConnectAsync()
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
            var contentHeaders = content.Headers.ToDictionary(item => item.Key, item => string.Join("; ", item.Value));

            Headers = contentHeaders
                .Concat(Headers.Where(header => !contentHeaders.ContainsKey(header.Key)))
                .ToDictionary(item => item.Key, item => item.Value);

            IStream stream = await MakeRequest("POST", path);

            var contentBytes = await content.ReadAsByteArrayAsync();
            var buffer = new ArraySegment<byte>(contentBytes);
            await stream.WriteAsync(buffer);

            return await GetResponse(stream);
        }
        #endregion

        #region Private methods
        private async Task<IStream> MakeRequest(string method, string path)
        {
            IDictionary<string, string> basicHeaders = new Dictionary<string, string>{
                {":method", method},
                {":scheme", _uri.Scheme},
                {":path", path},
                {":authority", $"{_uri.Host}:{_uri.Port}"},
            };

            var userHeaders = Headers.Where(header => !basicHeaders.ContainsKey(header.Key));

            var headers = basicHeaders
                .Concat(userHeaders)
                .Select(header => new HeaderField()
                {
                    Name = header.Key,
                    Value = header.Value
                });


            if (_connection == null)
            {
                await ConnectAsync();
            }

            IStream stream = await _connection.CreateStreamAsync(headers);

            return stream;
        }

        private async Task<Http2ResponseMessage> GetResponse(IStream stream)
        {
            IEnumerable<HeaderField> responseHeaders = new List<HeaderField>();
            List<byte> responseData = new List<byte>();

            responseHeaders = await stream.ReadHeadersAsync();

            byte[] buf = new byte[8192];
            while (true)
            {
                StreamReadResult res = await stream.ReadAsync(new ArraySegment<byte>(buf));
                if (res.EndOfStream)
                {
                    break;
                }
                responseData.AddRange(buf.Take(res.BytesRead));
            }

            if (stream.State != StreamState.Closed)
            {
                await stream.CloseAsync();
            }

            return new Http2ResponseMessage(responseHeaders.ToDictionary(item => item.Name, item => item.Value), responseData.ToArray());
        }
        #endregion

        #region IDisposable implementation
        public void Dispose()
        {
            _connection.CloseNow().Wait();
        }
        #endregion
    }
}
