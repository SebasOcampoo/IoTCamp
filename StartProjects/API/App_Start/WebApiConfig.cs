using API.App_Start;
using API.Models;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Http;
using WebJobs_Shared.DeviceManager;

namespace API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //Setup Dependency Injection
            var container = new UnityContainer();
            //container.RegisterType<ITokensRepository, TokensRepository>(new ContainerControlledLifetimeManager()); // Singleton

            string iotHubConnectionString = WebConfigurationManager.ConnectionStrings["Microsoft.Azure.IoTHub.ConnectionString.Service"].ConnectionString;
            container.RegisterType<ITokensRepository, TokensRepository>(new PerResolveLifetimeManager(), new InjectionConstructor(iotHubConnectionString));  // Singleton
            container.RegisterType<IDeviceValidator, STDeviceValidator>(new PerResolveLifetimeManager()); // Singleton
            container.RegisterType<IDeviceManager, IotHubDeviceManager>(new PerResolveLifetimeManager()); // Singleton

            config.DependencyResolver = new UnityResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
