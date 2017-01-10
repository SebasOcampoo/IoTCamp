using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJob_NotifyDevices.Model
{
    public interface IC2DMessageSender
    {
        Task SendCloudToDeviceMessageAsync(string deviceId, string message);

    }
}
