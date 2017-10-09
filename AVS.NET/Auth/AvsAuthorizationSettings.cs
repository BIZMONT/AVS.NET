using System;
using System.Collections.Generic;
using System.Text;

namespace AVS.NET.Auth
{
    public class AvsAuthorizationSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ProductId { get; set; }
        public string RedirectUri { get; set; }
        public string DeviceSerialNumber { get; set; }
    }
}
