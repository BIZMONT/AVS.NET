using Microsoft.AspNetCore.Mvc;
using AVS.Auth;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using AVS.NET.Auth;
using AVS.Auth.Requests;
using AVS.Auth.Tokens;

namespace AVS.WebServer.Controllers
{
    public class AvsAuthController : Controller
    {
        public AvsAuthorizationSettings AvsAuthorizationSettings { get; set; }

        public AvsAuthController(IOptions<AvsAuthorizationSettings> avsOptions)
        {
            AvsAuthorizationSettings = avsOptions.Value;
        }

        [Route("/auth/code")]
        public async Task<IActionResult> CodeGrantAuth(CodeGrantRequest model)
        {
            var avsAuth = new AvsCompanionSiteAuthorization(AvsAuthorizationSettings);

            var token = await avsAuth.AskForCodeGrantTokenAsync(model.Code);

            if(token.IsValid)
            {
                HttpContext.Session.SetString("AccessToken", token.AccessToken);
                HttpContext.Session.SetString("ExpiresDate", token.ExpiresDate.ToString());
                HttpContext.Session.SetString("RefreshToken", token.RefreshToken);
            }
            else
            {
                return Content("Access token is invalid");
            }

            return RedirectToAction("Index", "Home");
        }

        [Route("/auth/token")]
        public IActionResult ImplicitGrantAuth(ImplicitGrantRequest model)
        {
            var avsAuth = new AvsCompanionSiteAuthorization(AvsAuthorizationSettings);

            ImplicitGrantToken token = model;

            if (token.IsValid)
            {
                HttpContext.Session.SetString("AccessToken", token.AccessToken);
                HttpContext.Session.SetString("ExpiresDate", token.ExpiresDate.ToString());
            }
            else
            {
                return Content("Access token is invalid");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}