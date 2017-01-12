using CTDWeb.Models;
using Microsoft.Azure.Devices;
using Microsoft.WindowsAzure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebJobs_Shared.DeviceManager;

namespace CTDWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDeviceManager _deviceManager;
        public HomeController(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        public async Task<ActionResult> Index(string id)
        {
            // Dashboard

            HomeModel model = new HomeModel();

            // LOAD SETTING HERE

            // CHECK DEVICE STATUS HERE

            return View(model);
        }

        public ActionResult Insert()
        {
            // Home page

            InsertModel model = new InsertModel()
            {
                MAC1 = "",
                MAC2 = "",
                MAC3 = "",
                MAC4 = "",
                MAC5 = "",
                MAC6 = ""
            };

            var mac = Session["MAC"] as string;

            if (!string.IsNullOrEmpty(mac) && mac.Length == 12)
            {
                model.MAC1 = mac.Substring(0, 2);
                model.MAC2 = mac.Substring(2, 2);
                model.MAC3 = mac.Substring(4, 2);
                model.MAC4 = mac.Substring(6, 2);
                model.MAC5 = mac.Substring(8, 2);
                model.MAC6 = mac.Substring(10, 2);
            }

            return View(model);
        }

        public ActionResult About()
        {
            // About page
            ViewBag.Message = "Your application description page.";

            return View();
        }


        private async Task<Twin> SetDefaultStatus(string macAddress, string etag)
        {
            // SET DEVICE STATUS HERE
        }
    }
}