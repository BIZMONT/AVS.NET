namespace AVS.Auth.Requests
{
    public class CodeGrantRequest : AuthRequest
    {
        public string Code { get; set; }
    }
}
