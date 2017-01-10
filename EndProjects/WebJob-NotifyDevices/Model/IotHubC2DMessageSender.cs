using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJob_NotifyDevices.Model
{
    public class IotHubC2DMessageSender : IC2DMessageSender
    {
        private readonly string _connectionString;
        private ServiceClient _serviceClient;

        public IotHubC2DMessageSender()
        {   
            _connectionString = ConfigurationManager.ConnectionStrings["Microsoft.Azure.IoTHub.ConnectionString.Service"].ConnectionString;
            _serviceClient = ServiceClient.CreateFromConnectionString(_connectionString);
        }


        public async Task SendCloudToDeviceMessageAsync(string deviceId, string message)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            await _serviceClient.SendAsync(deviceId, commandMessage);
        }
    }
}
