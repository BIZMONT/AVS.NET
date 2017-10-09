using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVS.WebServer.Models
{
    public class AvsCodeGrandRequestModel
    {
        //[FromQuery(Name ="code")]
        public string Code { get; set; }

        //[FromQuery(Name ="state")]
        public string State { get; set; }

        //[FromQuery(Name ="scope")]
        public string Scope { get; set; }
    }
}
