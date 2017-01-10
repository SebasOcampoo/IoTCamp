using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJob_NotifyDevices.Model
{
    public interface INotificationServer
    {
        void connect();
        void notify(params object[] args);
        void disconnect();

    }
}
