using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AVS.WebServer.Controllers
{
    public class AvsAuthController : Controller
    {
        public IConfiguration Configuration { get; set; }

        public AvsAuthController(IConfiguration configs)
        {
            Configuration = configs;
        }

        [Route("/auth/code")]
        public IActionResult CodeAuth()
        {
            return View();
        }
    }
}