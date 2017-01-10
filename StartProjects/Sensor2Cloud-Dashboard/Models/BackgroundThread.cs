using CTDWeb.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace CTDWeb.Models
{
    public class BackgroundThread
    {
        public static void StartEHReceiver(EventHubSettings eventHubSettings, int partitionId)
        {
            var thread = new Thread(new ThreadStart(() => StartJob(eventHubSettings, partitionId)));
            thread.IsBackground = true;
            thread.Name = "EH" + eventHubSettings.name + "_Receiver_" + partitionId;
            thread.Start();
        }

        private static void StartJob(EventHubSettings eventHubSettings, int partitionId)
        {
            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(eventHubSettings.connectionString, eventHubSettings.name);
            EventHubConsumerGroup consumerGroup = eventHubClient.GetConsumerGroup(eventHubSettings.consumerGroup);

            IHubContext _hubContext;

            _hubContext = GlobalHost.ConnectionManager.GetHubContext<SensorHub>();

            Microsoft.ServiceBus.Messaging.EventHubReceiver receiver = null;

            //string initialOffset = "12900299552";
            //receiver = consumerGroup.CreateReceiver(state.ToString(), initialOffset);
            receiver = consumerGroup.CreateReceiver(partitionId.ToString(), DateTime.UtcNow);

            string[] newLine = new string[] { Environment.NewLine };

            while (true)
            {
                // Receive could fail, I would need a retry policy etc... 
                IEnumerable<EventData> messages = null;
                try
                {
                    messages = receiver.Receive(5);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ecc");
                }

                foreach (var message in messages)
                {
                    // Not a single JSON message: attempt to deserialize as array of messages

                    // Azure Stream Analytics Preview generates invalid JSON for some multi-values queries
                    // Workaround: turn concatenated json objects (ivalid JSON) into array of json objects (valid JSON)

                    //var readings = Encoding.Default.GetString(message.GetBytes()).Split(newLine, StringSplitOptions.None);

                    //foreach (var reading in readings)
                    //{
                    //    var messagePayload = JsonConvert.DeserializeObject<IDictionary<string, object>>(reading);
                    //    string send = JsonConvert.SerializeObject(messagePayload);
                    //    string id = messagePayload["id"].ToString();.            
                    //    // This is how we can access the Clients property in a static hub method or outside of the hub entirely
                    //    _hubContext.Clients.Group(id).addReading(send);
                    //}

                    // NOW WE CAN CONFIGURE ASA OUTPUT TO BE FORMATTED AS JSON ARRAY INSTEAD OF JSON OBJECTS SEPARATED BY NEWLINES
                    var messageString = Encoding.Default.GetString(message.GetBytes());
                   
                    try
                    {
                        var readings = JsonConvert.DeserializeObject<IList<IDictionary<string, object>>>(messageString);
                        foreach (var reading in readings)
                        {
                            string id = reading["id"].ToString();
                            // This is how we can access the Clients property in a static hub method or outside of the hub entirely
                            _hubContext.Clients.Group(id).addReading(reading);
                        }
                    } catch
                    {

                    }

                }
            }
        }
    }
}