using AVS.Core.Http2;
using System;
using System.Threading.Tasks;

namespace AVS.Core
{
    public class AvsClient : IDisposable
    {
        private static string _apiVersion = "v20160207";

        private string _accessToken;

        public string AccessToken
        {
            get
            {
                return _accessToken;
            }
            set
            {
                _accessToken = value;
                _http2Client.Headers["authorization"] = $"Bearer {_accessToken}";
            }
        }

        private Http2Client _http2Client;

        #region Constructors
        public AvsClient(Uri uri, string accessToken)
        {
            _accessToken = accessToken;
            _http2Client = new Http2Client(uri);
            _http2Client.Headers.Add("authorization", $"Bearer {AccessToken}");
        }
        #endregion

        public async Task<bool> TryEstablishConnectionAsync()
        {
            try
            {
                var response = await _http2Client.GetAsync($"/{_apiVersion}/directives", true);

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
