using Microsoft.Azure.Devices;
using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CTDWeb.Models
{
    public interface IAzureIoTHubService
    {
        Task SendCloudToDeviceMessageAsync(string deviceId, string message);
    }
}