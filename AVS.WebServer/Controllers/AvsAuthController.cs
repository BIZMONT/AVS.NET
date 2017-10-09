using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AVS.WebServer.Models;
using AVS.Auth;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AVS.WebServer.Controllers
{
    public class AvsAuthController : Controller
    {
        public AuthorizationSettings AvsAuthorizationSettings { get; set; }

        public AvsAuthController(IOptions<AuthorizationSettings> avsOptions)
        {
            AvsAuthorizationSettings = avsOptions.Value;
        }

        [Route("/auth/code")]
        public async Task<IActionResult> CodeAuth(AvsCodeGrandRequestModel model)
        {
            var avsAuth = new AvsCompanionSiteAuthorization(AvsAuthorizationSettings);

            var token = await avsAuth.AskForCodeGrantTokenAsync(model.Code);

            if(token.IsValid)
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