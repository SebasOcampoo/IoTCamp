using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CTDWeb.Models
{
    public class HomeModel
    {
        public string MAC { get; set; }
        public string Status { get; set; }
        public string API_URL { get; set; }
        public string BING_SPEECH_API_KEY { get; set; }
        public string LUIS_APP_ID { get; set; }
        public string LUIS_SUBSCRIPTION_ID { get; set; }
    }
}