using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJob_NotifyDevices.Model
{
    public class QueueReceivedMessage
    {
        public string MacAddress { get; set; }
        public string Timestamp { get; set; }
        public int MessagesCount { get; set; }
    }
}
