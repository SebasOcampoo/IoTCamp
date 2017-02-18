
Put your connection strings in the `Web.config` file:

```xml
<appSettings>
	<add key="Microsoft.ServiceBus.EventHubDevices" value="[EVENTHUB NAME]" />
	<add key="Microsoft.ServiceBus.EventHubDevices.PartitionCount" value="[EVENTHUB PARTITION COUNT]" />
	<add key="Microsoft.ServiceBus.EventHubDevices.ConsumerGroup" value="[EVENTHUB PRODUCTION CONSUMER GROUP]" />
	<add key="API.URL" value="[API URL]" />
	<add key="BING_SPEECH_API_KEY" value="[BING SPEECH API KEY]" />
    <add key="LUIS_APP_ID" value="[LUIS APP ID]" />
    <add key="LUIS_SUBSCRIPTION_ID" value="[LUIS SUBSCRIPTION ID]" />
</appSettings>
<connectionStrings>
	<add name="Microsoft.Azure.IoTHub.ConnectionString.Service" connectionString="[IOT HUB CONNECTION STRING]" />
	<add name="Microsoft.ServiceBus.ConnectionStringDevices" connectionString="[EVENTHUB CONNECTION STRING]" />
</connectionStrings>

```

Put your application insights instrumental key in the `ApplicationInsights.config` file

```xml
<InstrumentationKey>Your Application Insights key</InstrumentationKey>
```





  