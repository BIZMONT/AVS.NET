using AVS.Auth.Tokens;
using AVS.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace AVS.Auth
{
    public class AvsCompanionSiteAuthorization
    {
        private static string _amazonApiHost = "https://api.amazon.com";
        private static string _codeGrantAuthPath = "/auth/o2/token";
        private static string _authUrl = "https://www.amazon.com/ap/oa";

        #region Properties
        public static string AmazonApiHost
        {
            get
            {
                return _amazonApiHost;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentOutOfRangeException("API host can`t be empty");
                }
                _amazonApiHost = value;
            }
        }

        public static string CodeGrantAuthPath
        {
            get
            {
                return _codeGrantAuthPath;
            }
            set
            {
                _codeGrantAuthPath = value;
            }
        }

        public static string AuthorizationServerUrl
        {
            get
            {
                return _authUrl;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentOutOfRangeException("Authorization server URL can`t be empty");
                }
                _authUrl = value;
            }
        }


        public AuthorizationSettings Settings { get; set; }
        #endregion

        public AvsCompanionSiteAuthorization(AuthorizationSettings settings)
        {
            Settings = settings;
        }

        #region Public methods
        public virtual async Task<CodeGrantToken> AskForCodeGrantTokenAsync(string code)
        {
            var bodyParameters = new Dictionary<string, string>
                {
                    {"grant_type", "authorization_code"},
                    {"code", code},
                    {"client_id", Settings?.ClientId},
                    {"client_secret", Settings?.ClientSecret},
                    {"redirect_uri", Settings?.RedirectUri},
                };

            var content = new FormUrlEncodedContent(bodyParameters);

            CodeGrantToken token = await GetTokenFromAmazonAuthApiAsync(content);

            return token;
        }

        public virtual async Task<CodeGrantToken> RefreshCodeGrandTokenAsync(string refreshToken)
        {

            var bodyParameters = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"refresh_token",refreshToken},
                {"client_id", Settings?.ClientId},
                {"client_secret", Settings?.ClientSecret},
            };

            var content = new FormUrlEncodedContent(bodyParameters);

            CodeGrantToken token = await GetTokenFromAmazonAuthApiAsync(content);

            return token;
        }

        public string GetLwaConsentLink(AuthType authType, string state)
        {
            var scope = "alexa:all";

            var requestUri = new UriBuilder(_authUrl);

            NameValueCollection query = HttpUtility.ParseQueryString(requestUri.Query);
            query["client_id"] = Settings?.ClientId;
            query["scope"] = scope;
            query["scope_data"] = GenerateScopeDataJson(scope);
            query["response_type"] = authType.ToString().ToLower();
            query["state"] = state;
            query["redirect_uri"] = Settings?.RedirectUri;

            requestUri.Query = query.ToString();

            return requestUri.ToString();
        }
        #endregion

        #region Private methods
        private async Task<CodeGrantToken> GetTokenFromAmazonAuthApiAsync(FormUrlEncodedContent content)
        {
            CodeGrantToken accessToken = null;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_amazonApiHost);

                try
                {
                    var response = await httpClient.PostAsync(_codeGrantAuthPath, content);
                    var responesContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        accessToken = JsonConvert.DeserializeObject<CodeGrantToken>(responesContent);
                    }
                }
                catch (HttpRequestException)
                {
                    //TODO: Log connection error
                    throw;
                }
                catch
                {
                    //TODO: Log other errors
                    throw;
                }

                return accessToken;
            }
        }

        private string GenerateScopeDataJson(string scopeName)
        {
            var scopeData = new JObject
            {
                {
                    scopeName,
                    JObject.FromObject(new
                    {
                        productID = Settings?.ProductId,
                        productInstanceAttributes = new
                        {
                            deviceSerialNumber = Settings?.DeviceSerialNumber
                        }
                    })
                }
            };
            return Regex.Replace(scopeData.ToString(), @"\s+", string.Empty);
        }
        #endregion
    }

    public class AuthorizationSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ProductId { get; set; }
        public string RedirectUri { get; set; }
        public string DeviceSerialNumber { get; set; }
    }
}
