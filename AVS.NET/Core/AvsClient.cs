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
        public string AccessToken { get; }

        private Http2Client _http2Client;

        #region Constructors
        public AvsClient(Uri uri, string accessToken)
        {
            AccessToken = accessToken;
            _http2Client = new Http2Client(uri);
            _http2Client.Headers.Add("authorization", $"Bearer {AccessToken}");
        }
        #endregion

        public async Task<bool> EstablishConnectionAsync()
        {
            try
            {
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
