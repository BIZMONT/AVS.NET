using Microsoft.AspNetCore.Mvc;

namespace AVS.Auth.Requests
{
    public abstract class AuthRequest
    {
        [FromQuery(Name = "scope")]
        public string Scope { get; set; }

        [FromQuery(Name = "state")]
        public string State { get; set; }
    }
}
