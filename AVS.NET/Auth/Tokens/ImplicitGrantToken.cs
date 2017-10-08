using AVS.Auth.Requests;

namespace AVS.Auth.Tokens
{
    public class ImplicitGrantToken : AuthToken
    {
        public static implicit operator ImplicitGrantToken(ImplicitGrantRequest request)
        {
            return new ImplicitGrantToken
            {
                AccessToken = request.AccessToken,
                TokenType = request.TokenType,
                ExpiresIn = request.ExpiresIn
            };
        }
    }
}
