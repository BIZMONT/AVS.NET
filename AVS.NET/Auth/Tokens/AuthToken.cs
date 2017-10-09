using Newtonsoft.Json;
using System;

namespace AVS.Auth.Tokens
{
    public abstract class AuthToken
    {
        private int _expiresIn;

        [JsonProperty("access_token")]
        public string AccessToken { get; internal set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn
        {
            get
            {
                return _expiresIn;
            }
            internal set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("ExpiresIn value can`t be less than '0'");
                }

                _expiresIn = value;

                ExpiresDate = DateTime.Now.AddSeconds(_expiresIn);
            }
        }

        [JsonProperty("token_type")]
        public string TokenType { get; internal set; }

        public DateTime? ExpiresDate { get; internal set; }


        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(AccessToken) && ExpiresDate > DateTime.Now;
            }
        }
    }
}
