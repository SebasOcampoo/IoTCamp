using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus.Messaging;
using System.IO;
using System.Configuration;
using Microsoft.ServiceBus;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Practices.Unity;
using WebJob_NotifyDevices.Model;
using WebJobs_Shared.DeviceManager;

namespace WebJob_NotifyDevices
{
    class Program
    {
        public static void Main()
        {
            if (!VerifyConfiguration())
            {
                Console.ReadLine();
                return;
            }

            JobHostConfiguration config = new JobHostConfiguration();
            config.UseServiceBus();

            string iotHubConnectionString = ConfigurationManager.ConnectionStrings["Microsoft.Azure.IoTHub.ConnectionString.Service"].ConnectionString;

            string SIGNALR_HUB_NAME = "SensorHub";
            string SIGNALR_FUNCTION_NAME = "deviceStatusChanged";
            string SIGNALR_HUB_URL = ConfigurationManager.AppSettings["Dashboard.URL"].ToString();

            // DI configuration
            IUnityContainer unityContainer = new UnityContainer();
            unityContainer.RegisterType<IC2DMessageSender, IotHubC2DMessageSender>(new PerResolveLifetimeManager()); // singleton
            unityContainer.RegisterType<IDeviceManager, IotHubDeviceManager>(new PerResolveLifetimeManager(), new InjectionConstructor(iotHubConnectionString));
            unityContainer.RegisterType<INotificationServer, SignalRNotificationServer>(new PerResolveLifetimeManager(),
                new InjectionConstructor(SIGNALR_HUB_URL, SIGNALR_HUB_NAME, SIGNALR_FUNCTION_NAME)
            );

            config.JobActivator = new DIJobActivator(unityContainer);
            config.NameResolver = new AppSettingsNameResolver();

            //// Send test message in the queue - DEBUGGING
            //var client = QueueClient.CreateFromConnectionString(ConfigurationManager.ConnectionStrings["AzureWebJobsServiceBus"].ConnectionString, ConfigurationManager.AppSettings["queueName"].ToString());
            //var message = new BrokeredMessage(
            //    Newtonsoft.Json.JsonConvert.SerializeObject(new QueueReceivedMessage()
            //    {
            //        MacAddress = "2",
            //        MessagesCount = 15,
            //        Timestamp = DateTime.Now.ToString()
            //    }
            //)); // disable device 2
            //client.Send(message);

            JobHost host = new JobHost(config);
            host.RunAndBlock();
        }

        private static bool VerifyConfiguration()
        {
            string webJobsDashboard = ConfigurationManager.ConnectionStrings["AzureWebJobsDashboard"].ConnectionString;
            string webJobsStorage = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString;
            string servicesBusConnectionString = ConfigurationManager.ConnectionStrings["AzureWebJobsServiceBus"].ConnectionString;
            string iotHubConnectionString = ConfigurationManager.ConnectionStrings["Microsoft.Azure.IoTHub.ConnectionString.Service"].ConnectionString;

            bool configOK = true;
            if (string.IsNullOrWhiteSpace(webJobsDashboard) || string.IsNullOrWhiteSpace(webJobsStorage))
            {
                configOK = false;
                Console.WriteLine("Please add the Azure Storage account credentials in App.config");
            }
            if (string.IsNullOrWhiteSpace(servicesBusConnectionString))
            {
                configOK = false;
                Console.WriteLine("Please add your Service Bus connection string in App.config");
            }

            if (string.IsNullOrWhiteSpace(iotHubConnectionString))
            {
                configOK = false;
                Console.WriteLine("Please add your Iot Hub connection string in App.config");
            }

            return configOK;
        }
    }

    public class AppSettingsNameResolver : INameResolver
    {
        public string Resolve(string name)
        {
            return ConfigurationManager.AppSettings[name].ToString();
        }
    }

    public class DIJobActivator : IJobActivator
    {
        private readonly IUnityContainer _container;

        public DIJobActivator(IUnityContainer container)
        {
            _container = container;
        }

        public T CreateInstance<T>()
        {
            return _container.Resolve<T>();
        }
    }
}
