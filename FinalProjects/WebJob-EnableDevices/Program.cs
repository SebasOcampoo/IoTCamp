using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Practices.Unity;
using WebJobs_Shared.DeviceManager;
using System.Configuration;

namespace WebJob_EnableDevices
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

            string iotHubConnectionString = ConfigurationManager.ConnectionStrings["Microsoft.Azure.IoTHub.ConnectionString.Service"].ConnectionString;

            JobHostConfiguration config = new JobHostConfiguration();
            config.UseTimers();

            // DI configuration
            IUnityContainer unityContainer = new UnityContainer();
            unityContainer.RegisterType<IDeviceManager, IotHubDeviceManager>(new PerResolveLifetimeManager(), new InjectionConstructor(iotHubConnectionString));

            config.JobActivator = new DIJobActivator(unityContainer);

            JobHost host = new JobHost(config);
            host.RunAndBlock();
        }

        private static bool VerifyConfiguration()
        {
            string webJobsDashboard = ConfigurationManager.ConnectionStrings["AzureWebJobsDashboard"].ConnectionString;
            string webJobsStorage = ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString;
            string iotHubConnectionString = ConfigurationManager.ConnectionStrings["Microsoft.Azure.IoTHub.ConnectionString.Service"].ConnectionString;

            bool configOK = true;
            if (string.IsNullOrWhiteSpace(webJobsDashboard) || string.IsNullOrWhiteSpace(webJobsStorage))
            {
                configOK = false;
                Console.WriteLine("Please add the Azure Storage account credentials in App.config");
            }

            if (string.IsNullOrWhiteSpace(iotHubConnectionString))
            {
                configOK = false;
                Console.WriteLine("Please add your Iot Hub connection string in App.config");
            }

            return configOK;
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
