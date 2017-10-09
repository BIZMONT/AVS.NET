using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using AVS.Auth;
using AVS.Enums;
using Microsoft.Extensions.Options;
using AVS.WebServer.Models;

namespace AVS.WebServer.Controllers
{
    public class HomeController : Controller
    {
        public AuthorizationSettings AvsAuthorizationSettings { get; set; }

        public HomeController(IOptions<AuthorizationSettings> avsOptions)
        {
            AvsAuthorizationSettings = avsOptions.Value;
        }

        public IActionResult Index()
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            var accessTokenExpires = HttpContext.Session.GetString("ExpiresDate");

            DateTime.TryParse(accessTokenExpires, out DateTime accessTokenExpiresDate);

            HomeViewModel model = new HomeViewModel();

            if (string.IsNullOrEmpty(accessToken) || accessTokenExpiresDate < DateTime.Now)
            {
                HttpContext.Session.Remove("AccessToken");
                HttpContext.Session.Remove("Expires");

                var avsAuth = new AvsCompanionSiteAuthorization(AvsAuthorizationSettings);

                model.AuthorizationLinkUrl = avsAuth.GetLwaConsentLink(AuthType.Code, "1234");
            }
            else
            {
                model.IsAuthorized = true;
            }

            return View(model);
        }
    }
}