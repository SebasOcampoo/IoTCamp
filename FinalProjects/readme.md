Sensor2Cloud projects' folder.

First of we are going to create the Azure architecture:

* Login in the [Azure Portal](https://portal.azure.com) 
* Follow [this](../Docs/cloud_architecture_configuration.md) instructions

{% include_relative ../Docs/cloud_architecture_configuration %}

At last we can publish our services, follow markdown files in this order:
* [Sensor2Cloud-Dashboard](Sensor2Cloud-Dashboard) 
* [API](API) 
* [WebJob-NotifyDevices](WebJob-NotifyDevices) 
* [WebJob-EnableDevices](WebJob-EnableDevices)
