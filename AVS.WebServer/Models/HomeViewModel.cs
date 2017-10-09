using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVS.WebServer.Models
{
    public class HomeViewModel
    {
        public bool IsAuthorized { get; set; }
        public string AuthorizationLinkUrl { get; set; }
    }
}
