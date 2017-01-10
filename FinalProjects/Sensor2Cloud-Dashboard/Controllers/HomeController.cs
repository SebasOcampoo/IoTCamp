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
            HomeModel model = new HomeModel();

            model.API_URL = CloudConfigurationManager.GetSetting("API.URL");
            model.BING_SPEECH_API_KEY = CloudConfigurationManager.GetSetting("BING_SPEECH_API_KEY");
            model.LUIS_APP_ID = CloudConfigurationManager.GetSetting("LUIS_APP_ID");
            model.LUIS_SUBSCRIPTION_ID = CloudConfigurationManager.GetSetting("LUIS_SUBSCRIPTION_ID");

            if (id != null)
            {
                model.MAC = id.ToUpper();
                //Store the mac address to a session
                Session["MAC"] = id;

                var twin = await _deviceManager.GetTwin(id);
                string status = string.Empty;

                if (twin == null) // probably device not registered
                {
                    model.Status = status;
                }
                else
                {
                    try
                    {
                        status = twin.Properties.Desired["status"];
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        // this device has not status property
                        twin = await SetDefaultStatus(id, twin.ETag);
                        status = twin.Properties.Desired["status"];
                    }
                    finally
                    {
                        model.Status = status;
                    }
                }
            }
            else
            {
                return RedirectToAction("Insert", "Home");
            }

            return View(model);
        }

        public ActionResult Insert()
        {
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
            ViewBag.Message = "Your application description page.";

            return View();
        }


        private async Task<Twin> SetDefaultStatus(string macAddress, string etag)
        {
            Console.WriteLine("no status -> enabled");

            var path = new
            {
                properties = new
                {
                    desired = new
                    {
                        status = "enabled"
                    }
                }
            };

            var twin = await _deviceManager.UpdateTwin(macAddress, JsonConvert.SerializeObject(path), etag);
            return twin;
        }
    }
}