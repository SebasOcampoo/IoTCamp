Put your connection strings in the `Web.config` file:

```
<add name="Sensor2CloudContext" connectionString="<SQL DB ConnectionString>" providerName="System.Data.SqlClient" />
<add name="Microsoft.Azure.IoTHub.ConnectionString.Service" connectionString="<IoTHub ConnectionString>" />
```

Put your application insights instrumental key in the `ApplicationInsights.config` file

```
<InstrumentationKey>Your Application Insights key</InstrumentationKey>
```