
Put your connection strings in the `App.config` file:

```xml
<connectionStrings>
    <add name="AzureWebJobsDashboard" connectionString="<Storage account connection string>" />
    <add name="AzureWebJobsStorage" connectionString="<Storage account connection string>" />
    <add name="AzureWebJobsServiceBus" connectionString="<ServiceBus account connection string>" />
    <add name="Microsoft.Azure.IoTHub.ConnectionString.Service" connectionString="<IotHub account connection string>" /> 
</connectionStrings>

```  

Update the `queuename` app setting in the `App.config` file:

```xml
<appSettings>
    <add key="queueName" value="<Put your queue's name here>" />
</appSettings>
```