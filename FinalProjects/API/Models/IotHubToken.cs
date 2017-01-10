using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class IotHubToken: IToken
    {
        public string DeviceID { get; set; }
        public string Token { get; set; }
        public string Status { get; set; }
    }
}