# CLOUD ARCHITECTURE

REQUIREMENTS: you must be logged in your Azure subscription

At the beginning you need to create all the following Azure resources:

*** TODO: CHECK IOTHUB PREVIEW version

* `Azure IotHub Preview version` ([official guide](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-create-through-portal))
* `Azure StreamAnalytics` ([official guide](https://docs.microsoft.com/en-us/azure/stream-analytics/stream-analytics-create-a-job))
* `Azure WebApp` ([official guide](https://docs.microsoft.com/en-us/azure/app-service-web/app-service-web-how-to-create-a-web-app-in-an-ase))
    * to deploy the *dashboard*
* `Azure ApiApp` ([overview](https://docs.microsoft.com/en-us/azure/app-service-api/app-service-api-apps-why-best-platform) | [example](https://docs.microsoft.com/en-us/azure/app-service-api/app-service-api-dotnet-get-started))
    * to deploy the *registration api*
    * use the same App Service Plan (more info [here](https://docs.microsoft.com/en-us/azure/app-service/app-service-value-prop-what-is)) of the WebApp
* `Azure SQL Database`  ([official guide](https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started)
                        | [create more servers](https://github.com/Microsoft/azure-docs/blob/master/articles/sql-database/sql-database-create-servers.md)
                        | [create more dbs](https://github.com/Microsoft/azure-docs/blob/master/articles/sql-database/sql-database-create-databases.md)
                        ) 
* `Azure EventHub` ([official guide](https://docs.microsoft.com/en-us/azure/event-hubs/event-hubs-csharp-ephcs-getstarted))
* `Azure ServiceBus Queue` ([official guide](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-dotnet-get-started-with-queues#2-create-a-queue-using-the-azure-portal))
    * [here](https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-queues-topics-subscriptions) you can find a service overview


Now we're going to create the cloud infrastructure connecting our services.

```
ARCHITECTURE IMAGE
```

## Configure IoTHub 

* Shared access policies 
    * api
    * webapp
    * webjob
    * streamAnalytics

* Messaging Endpoint
    * Consumer Group
        * streamAnalytics

## Configure EventHub
    * NS Shared access policies 
        * webapp
        * streamAnalytics
    * EH Consumer Group
        * local
        * website


## Configure SQL Database



## Configure ServiceBus Queue
    * NS Shared access policies 
        * webjob
        * streamAnalytics


## Configure StreamAnalytics

```
IMAGE
```

### Input
    * IoTHub (named IoTHubDM)
Images and documentation.

### Output
    * EventHub (named ehOut)
    * ServiceBus Queue (named Queue)
    * SQL Database (named SQL)

### Query
You can find the final query [here](../Utilities/StreamAnalyticsQuery.sql).

{% include_relative ../Utilities/StreamAnalyticsQuery.sql %}




