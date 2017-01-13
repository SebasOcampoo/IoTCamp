using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using WebJob_NotifyDevices.Model;
using Newtonsoft.Json;
using WebJobs_Shared.DeviceManager;
using System.Configuration;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Azure.Devices;

namespace WebJob_NotifyDevices
{
    public class Functions
    {
        public int Threshold
        {
            get
            {
                return int.Parse(ConfigurationManager.AppSettings["threshold"]);
            }
        }

        private readonly IC2DMessageSender _messageSender;
        private readonly IDeviceManager _deviceManager;

        private readonly INotificationServer _notificationServer;


        public Functions(IC2DMessageSender messageSender, IDeviceManager deviceManager, INotificationServer notificationServer)
        {
            _messageSender = messageSender;
            _deviceManager = deviceManager;
            _notificationServer = notificationServer;
            _notificationServer.connect();
        }


        // Triggered when service bus receive message in a queue 
        public async void SBQueue2IotHubDevice(
            [ServiceBusTrigger("%queueName%")] string message,
            TextWriter log)
        {
            // WRITE THE APPLICATION LOGIC TO CHANGE THE DEVICE STATUS HERE
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

        private async Task<Twin> Enable(string macAddress, string etag)
        {
            Console.WriteLine("warning -> enabled");

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

            // notify device status change -- parameters: device_id, new_state
            _notificationServer.notify(macAddress, "enabled");

            var twin = await _deviceManager.UpdateTwin(macAddress, JsonConvert.SerializeObject(path), etag);
            return twin;
        }

        private async Task<Twin> Disable(string macAddress, string etag)
        {
            Console.WriteLine("warning -> disabled");

            await _messageSender.SendCloudToDeviceMessageAsync(macAddress, "disabled");

            var path = new
            {
                properties = new
                {
                    desired = new
                    {
                        status = "disabled"
                    }
                }
            };

            // notify device status change -- parameters: device_id, new_state
            _notificationServer.notify(macAddress, "disabled");

            var twin = await _deviceManager.UpdateTwin(macAddress, JsonConvert.SerializeObject(path), etag);
            await _deviceManager.disable(macAddress);

            return twin;
        }

        private async Task<Twin> SetWarning(string macAddress, string etag)
        {
            Console.WriteLine("enabled -> warning");

            await _messageSender.SendCloudToDeviceMessageAsync(macAddress, "warning");

            var path = new
            {
                properties = new
                {
                    desired = new
                    {
                        status = "warning"
                    }
                }
            };

            // notify device status change -- parameters: device_id, new_state
            _notificationServer.notify(macAddress, "warning");

            var twin = await _deviceManager.UpdateTwin(macAddress, JsonConvert.SerializeObject(path), etag);
            return twin;
        }
    }
}