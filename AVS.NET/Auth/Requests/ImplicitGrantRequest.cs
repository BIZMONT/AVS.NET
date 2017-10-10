using Microsoft.AspNetCore.Mvc;

namespace AVS.Auth.Requests
{
    public class ImplicitGrantRequest : AuthRequest
    {
        [FromQuery(Name = "access_token")]
        public string AccessToken { get; set; }

        [FromQuery(Name = "token_type")]
        public string TokenType { get; set; }

        [FromQuery(Name = "expires_in")]
        public int ExpiresIn { get; set; }

    }
}
