
Put your connection strings in the `Web.config` file:

```
<appSettings>
	<add key="Microsoft.ServiceBus.EventHubDevices" value="<EventHub name>" />
	<add key="Microsoft.ServiceBus.EventHubDevices.PartitionCount" value="<EventHub partition count>" />
	<add key="Microsoft.ServiceBus.EventHubDevices.ConsumerGroup" value="<EventHub production consumer group>" />
	<add key="API.URL" value="<API Url>" />
	<add key="BING_SPEECH_API_KEY" value="<BING SPEECH API KEY>" />
    <add key="LUIS_APP_ID" value="<LUIS APP ID>" />
    <add key="LUIS_SUBSCRIPTION_ID" value="<LUIS SUBSCRIPTION ID>" />
</appSettings>
<connectionStrings>
	<add name="Microsoft.Azure.IoTHub.ConnectionString.Service" connectionString="<IotHub ConnectionString>" />
	<add name="Microsoft.ServiceBus.ConnectionStringDevices" connectionString="<EventHub ConnectionString>" />
</connectionStrings>

```

Put your application insights instrumental key in the `ApplicationInsights.config` file

```
<InstrumentationKey>Your Application Insights key</InstrumentationKey>
```





  