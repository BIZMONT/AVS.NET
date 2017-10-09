using Newtonsoft.Json;

namespace AVS.Auth.Tokens
{
    public class CodeGrantToken : AuthToken
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

    }
}
