
Put your connection strings in the `App.config` file:

```xml
<connectionStrings>
    <add name="AzureWebJobsDashboard" connectionString="<Storage account connection string>" />
    <add name="AzureWebJobsStorage" connectionString="<Storage account connection string>" />
    <add name="Microsoft.Azure.IoTHub.ConnectionString.Service" connectionString="<IotHub account connection string>" />
</connectionStrings>

```  