using Microsoft.AspNetCore.Mvc;

namespace AVS.Auth.Requests
{
    public class CodeGrantRequest : AuthRequest
    {
        [FromQuery(Name = "code")]
        public string Code { get; set; }
    }
}
