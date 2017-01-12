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
            // ENABLE DEVICES HERE
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
            // ENABLE ALL DEVICES TWIN HERE
        }
    }
}
