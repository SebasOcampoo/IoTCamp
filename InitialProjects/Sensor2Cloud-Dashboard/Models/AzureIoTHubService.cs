using Microsoft.Azure.Devices;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTDWeb.Models
{
    public class AzureIoTHubService : IAzureIoTHubService
    {
        private ServiceClient _serviceClient;

        public AzureIoTHubService(string iothubConnectionString)
        {
            _serviceClient = ServiceClient.CreateFromConnectionString(iothubConnectionString);
        }

        public async Task SendCloudToDeviceMessageAsync(string deviceId, string message)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            await _serviceClient.SendAsync(deviceId, commandMessage);
        }
    }
}