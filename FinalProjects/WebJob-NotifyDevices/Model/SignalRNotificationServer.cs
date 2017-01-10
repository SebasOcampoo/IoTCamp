using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebJob_NotifyDevices.Model
{
    class SignalRNotificationServer : INotificationServer
    {
        private readonly string _methodName;
        private readonly string _serverURL;
        private readonly string _hubname;

        private HubConnection signalRConnection = null;
        private IHubProxy signalRHub = null;

        public SignalRNotificationServer(string serverURL, string hubname, string methodName)
        {
            _serverURL = serverURL;
            _hubname = hubname;
            _methodName = methodName;
        }

        public void connect()
        {
            // set SignalR connection
            signalRConnection = new HubConnection(_serverURL);
            //Make proxy to hub based on hub name on server
            signalRHub = signalRConnection.CreateHubProxy(_hubname);
            signalRConnection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error opening the connection:{0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine("Connected");
                }

            }).Wait();
        }

        public void disconnect()
        {
            if(signalRConnection != null)
                signalRConnection.Stop();
        }

        public void notify(params object[] args)
        {
            signalRHub.Invoke<string>(_methodName, args).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("There was an error calling send: {0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine(task.Result);
                }
            });
        }
    }
}
