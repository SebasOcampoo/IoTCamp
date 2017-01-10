using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using System.Web.UI.WebControls;
using System.Diagnostics;
using Microsoft.WindowsAzure;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using CTDWeb.Models;
using System.Web.Configuration;

namespace CTDWeb
{
    public struct EventHubSettings
    {
        public string name { get; set; }
        public string connectionString { get; set; }
        public string consumerGroup { get; set; }
        public int partitionCount { get; set; }
    }

    public struct GlobalSettings
    {
        public bool ForceSocketCloseOnUserActionsTimeout { get; set; }
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        EventHubSettings eventHubDevicesSettings;
        public static GlobalSettings globalSettings;

        protected void Application_Start()
        {
            RouteTable.Routes.MapHttpRoute(
               name: "DefaultApi",
               routeTemplate: "api/{controller}/{id}",
               defaults: new { id = System.Web.Http.RouteParameter.Optional }
           );

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Read connectiong strings and Event Hubs names from app.config file
            GetAppSettings();

            // Create EventProcessorHost clients
            CreateEventProcessorHostClient(ref eventHubDevicesSettings);

        }

        protected void Application_End(Object sender, EventArgs e)
        {
            Trace.TraceInformation("Unregistering EventProcessorHosts");
        }

        private void GetAppSettings()
        {
            try
            {
                globalSettings.ForceSocketCloseOnUserActionsTimeout =
                    CloudConfigurationManager.GetSetting("ForceSocketCloseOnUserActionsTimeout") == "true";
            }
            catch (Exception)
            {
            }

            // Read settings for Devices Event Hub
            eventHubDevicesSettings.connectionString = WebConfigurationManager.ConnectionStrings["Microsoft.ServiceBus.ConnectionStringDevices"].ConnectionString;
            eventHubDevicesSettings.name = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.EventHubDevices").ToLowerInvariant();
            eventHubDevicesSettings.partitionCount = int.Parse(CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.EventHubDevices.PartitionCount"));

            if (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")))
            {
                // Assume we are running local: use different consumer groups to avoid colliding with a cloud instance
                eventHubDevicesSettings.consumerGroup = "local";
            }
            else
            {
                eventHubDevicesSettings.consumerGroup = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.EventHubDevices.ConsumerGroup");
            }
        }


        private void CreateEventProcessorHostClient(ref EventHubSettings eventHubSettings)
        {
            Trace.TraceInformation("Creating EventProcessorHost: {0}, {1}, {2}", this.Server.MachineName, eventHubSettings.name, eventHubSettings.consumerGroup);

            for (int i = 0; i < eventHubSettings.partitionCount; i++)
            {
                BackgroundThread.StartEHReceiver(eventHubSettings, i);
            }
        }
    }
}
