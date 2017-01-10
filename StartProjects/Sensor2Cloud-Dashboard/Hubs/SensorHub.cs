using CTDWeb.App_Start;
using CTDWeb.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CTDWeb.Hubs
{
    public class SensorHub : Hub
    {
        public void sendToUser(string deviceId, string reading)
        {
            Clients.Group(deviceId).addReading(reading);
        }

        public Task sendToDevice(string deviceId, string message)
        {
            IUnityContainer ccc = UnityConfig.GetConfiguredContainer();
            IAzureIoTHubService iotHubService = ccc.Resolve<IAzureIoTHubService>();
            return iotHubService.SendCloudToDeviceMessageAsync(deviceId, message);
        }

        public Task startReading(string deviceId)
        {
            return Groups.Add(Context.ConnectionId, deviceId);
        }

        public Task stopReading(string deviceId)
        {
            return Groups.Remove(Context.ConnectionId, deviceId);
        }

        public void deviceStatusChanged(string deviceId, string newStatus)
        {
            //Console.WriteLine(deviceId);
            Clients.Group(deviceId).deviceStatusChanged(newStatus);
        }
    }
}