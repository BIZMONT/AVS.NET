using AVS.Core.Http2;
using Http2.Hpack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AVS.Core
{
    public class AvsClient : IDisposable
    {
        private string _apiVersion;
        public string AccessToken { get; set; }

        private Http2Client _http2Client;

        #region Constructors
        public AvsClient(Uri url) : this(url, string.Empty) { }

        public AvsClient(Uri uri, string accessToken)
        {
            AccessToken = accessToken;
            _http2Client = new Http2Client(uri);
        }
        #endregion

        public async Task<bool> EstablishConnectionAsync()
        {
            try
            {
                await _http2Client.ConnectAsync();

                _http2Client.Headers.Clear();
                _http2Client.Headers.Add(new HeaderField() { Name = "authorization", Value = $"Bearer {AccessToken}" });

                var response = await _http2Client.GetAsync($"/{_apiVersion}/directives");

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }

        }

        public void Dispose()
        {
            _http2Client.Dispose();
        }
    }
}
