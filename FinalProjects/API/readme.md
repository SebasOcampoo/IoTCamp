Put your connection strings in the `Web.config` file:

```xml
<add name="Sensor2CloudContext" connectionString="[SQL DB CONNECTIONSTRING]" providerName="System.Data.SqlClient" />
<add name="Microsoft.Azure.IoTHub.ConnectionString.Service" connectionString="[IOT HUB CONNECTION STRING]" />
```

Put your application insights instrumental key in the `ApplicationInsights.config` file

```xml
<InstrumentationKey>Your Application Insights key</InstrumentationKey>
```