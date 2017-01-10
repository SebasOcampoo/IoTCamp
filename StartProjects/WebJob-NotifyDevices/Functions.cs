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
            QueueReceivedMessage m = null;

            try
            {
                m = JsonConvert.DeserializeObject<QueueReceivedMessage>(message);
            }
            catch (Exception)
            {
                log.WriteLine("Exception JSONConverter: " + message);
            }

            if (m != null)
            {
                log.WriteLine("SBQueue2IotHubDevice: " + m.MacAddress);

                //var slowDownMessage = new C2DMessage()
                //{
                //    Type = "SlowDown",
                //    Message = m.MessagesCount + " messages in the last hour",
                //    TimeStamp = m.Timestamp
                //};

                var twin = await _deviceManager.GetTwin(m.MacAddress);
                if (twin != null)
                {
                    try
                    {
                        var status = twin.Properties.Desired["status"];


                        Console.WriteLine("Status {0}", status, Formatting.Indented);

                        // Max number of messages only for non demo devices

                        if (twin.Tags.Contains("demo") == false)
                        {

                            // State machine

                            if (Convert.ToString(status) == "enabled" && m.MessagesCount > Threshold)
                            {
                                twin = await SetWarning(m.MacAddress, twin.ETag);
                            }
                            else if (Convert.ToString(status) == "warning" && m.MessagesCount > Threshold)
                            {
                                twin = await Disable(m.MacAddress, twin.ETag);
                            }
                            else if (Convert.ToString(status) == "warning" && m.MessagesCount < Threshold)
                            {
                                twin = await Enable(m.MacAddress, twin.ETag);
                            }
                        }

                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        // this device has not status property
                        twin = await SetDefaultStatus(m.MacAddress, twin.ETag);
                    }
                    catch (DeviceNotFoundException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

            }
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