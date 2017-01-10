using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using WebJobs_Shared.DeviceManager;
using Newtonsoft.Json;

namespace WebJob_EnableDevices
{
    public class Functions
    {
        private readonly IDeviceManager _deviceManager;

        public Functions(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }

        // Triggers every day at 00:00:00
        public async void CronJob([TimerTrigger("0 0 0 * * *")] TimerInfo timer, TextWriter log)
        {
            await _deviceManager.enableAll();
            EnableAllStatus();
        }

        // --- TEST enable devices ---
        // Runs once every 30 seconds
        //public async void TimerJob([TimerTrigger("00:00:30")] TimerInfo timer)
        //{
        //    await _deviceManager.enableAll();
        //    EnableAllStatus();
        //}
        // ------

        private async void EnableAllStatus()
        {
            var query = _deviceManager.CreateQuery("SELECT * FROM devices WHERE properties.desired.status IN ['disabled', 'warning']", 1000);

            while (query.HasMoreResults)
            {
                var page = await query.GetNextAsTwinAsync();
                foreach (var twin in page)
                {
                    var patch = new
                    {
                        properties = new
                        {
                            desired = new
                            {
                                status = "enabled"
                            }
                        }
                    };

                    await _deviceManager.UpdateTwin(twin.DeviceId, JsonConvert.SerializeObject(patch), twin.ETag);
                }
            }
        }
    }
}
