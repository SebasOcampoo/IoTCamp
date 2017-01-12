This folder contains the incomplete version of the Sensors2Cloud web application.

The solution has got 5 projects:
* Sensor2Cloud-Dashboard: [ASP.NET MVC](https://www.asp.net/mvc) web site that shows data coming from the devices in a dashboard
* API: [ASP.NET Web API](https://www.asp.net/web-api) project that expose some REST services that the device can call in order to register itself to Sensors2Cloud
* WebJob-EnableDevices: C# Console Application deployed as [Azure Web Job](https://goo.gl/xNK1Xz) that every day at midnight enable all the devices that are disabled in the IoT Hub
* WebJob-NotifyDevices: C# Console Application deployed as [Azure Web Job](https://goo.gl/xNK1Xz) that change the status of the devices that exceed the maximum number of messages allowed
* WebJobs-Shared: C# Class Library containing shared classes between the other web jobs projects

You must complete the code in order to have the application working, following the instructions below.


# 1. Sensor2Cloud-Dashboard project


The .cs files in all the ASP.NET MVC projects like this one, contain the server side logic of the web application and its goal is to provide the web pages (.cshtml files) to the user.

The main class of this project is *HomeController.cs*, which contains some public methods called actions. Each action provide to the user browser a specific web page.
For example the *Index* method of the *HomeController* return the web page *Index.cshtml* browsing the url `http://<domain>/Home/Index`.


In the next steps we will fill the code of the *HomeController.cs* class.

Load the settings for the configuration file of the project at the beginning of the *Index* method:


```cs
    // Load settings from web.config
    model.API_URL = CloudConfigurationManager.GetSetting("API.URL");
    model.BING_SPEECH_API_KEY = CloudConfigurationManager.GetSetting("BING_SPEECH_API_KEY");
    model.LUIS_APP_ID = CloudConfigurationManager.GetSetting("LUIS_APP_ID");
    model.LUIS_SUBSCRIPTION_ID = CloudConfigurationManager.GetSetting("LUIS_SUBSCRIPTION_ID");
```

We will see later what kind of settings they are.

Check if the device is registerd and its status that is saved to the IoT Hub [device twin](https://goo.gl/DwvBHf), using the code:

```cs
if (id != null)
{
    model.MAC = id.ToUpper();

    //Store the mac address to a session
    Session["MAC"] = id;

    // Retrieve device twin from Iot Hub
    var twin = await _deviceManager.GetTwin(id);
    string status = string.Empty;

    // Check if device is registered
    if (twin == null)
    {
        // Probably device not registered

        model.Status = status;
    }
    else
    {
        // Device registered

        try
        {
            // Check the device status
            status = twin.Properties.Desired["status"];
        }
        catch (ArgumentOutOfRangeException ex)
        {
            // This device has not status property
            twin = await SetDefaultStatus(id, twin.ETag);
            status = twin.Properties.Desired["status"];
        }
        finally
        {
            model.Status = status;
        }
    }
}
else
{
    // Redirect to the home page
    return RedirectToAction("Insert", "Home");
}
```

At the end, the *Index* method should be like this:


```cs
public async Task<ActionResult> Index(string id)
{
    // Dashboard

    HomeModel model = new HomeModel();

    // Load settings from web.config
    model.API_URL = CloudConfigurationManager.GetSetting("API.URL");
    model.BING_SPEECH_API_KEY = CloudConfigurationManager.GetSetting("BING_SPEECH_API_KEY");
    model.LUIS_APP_ID = CloudConfigurationManager.GetSetting("LUIS_APP_ID");
    model.LUIS_SUBSCRIPTION_ID = CloudConfigurationManager.GetSetting("LUIS_SUBSCRIPTION_ID");


    if (id != null)
    {
        model.MAC = id.ToUpper();

        //Store the mac address to a session
        Session["MAC"] = id;

        // Retrieve device twin from Iot Hub
        var twin = await _deviceManager.GetTwin(id);
        string status = string.Empty;

        // Check if device is registered
        if (twin == null)
        {
            // Probably device not registered

            model.Status = status;
        }
        else
        {
            // Device registered

            try
            {
                // Check the device status
                status = twin.Properties.Desired["status"];
            }
            catch (ArgumentOutOfRangeException ex)
            {
                // This device has not status property
                twin = await SetDefaultStatus(id, twin.ETag);
                status = twin.Properties.Desired["status"];
            }
            finally
            {
                model.Status = status;
            }
        }
    }
    else
    {
        // Redirect to the home page
        return RedirectToAction("Insert", "Home");
    }

    return View(model);
}
```

If a device is registered to the IoT Hub but it has not the *status* property in its twin, we must set it adding the following method in the *HomeController* class:

```cs
private async Task<Twin> SetDefaultStatus(string macAddress, string etag)
{
    // Set the default status of a device

    Console.WriteLine("no status -> enabled");

    // The path object define the new status of the device
    var path = new
    {
        properties = new
        {
            desired = new
            {
                status = "enabled"
            }
        }
    };

    // Update the device's status to the IoT Hub
    var twin = await _deviceManager.UpdateTwin(macAddress, JsonConvert.SerializeObject(path), etag);
    return twin;
}
```

Note that the new twin of the device is described as JSON object.


In this tutorial we don't change the html code of the websites, because the main goal of the hands on lab is to understand how to manage the devices registered in the IoT Hub.  


## 1.2 Restore project dependencies

Now that we completed the server side code of the dashboard, we must restore the missing libraries (.dll) throught [NuGet](https://www.nuget.org/) package manager.

1. Right click on the Visual Studio project
2. Click to *Manage NuGet Packages*
3. Click to *Restore* button on the page that will be opened




# 2. WebJob-EnableDevices

A device can be disabled if reach the defined daily quota of messages it can send to the IoT Hub.
So we need to check the status of each device registered in the IoT Hub at midnight and enable the previous disabled ones.  

This project contains a small C# program that is triggered by a CRON expression and do exacly what we need.   
In Azure this is possible through a feature of [Azure Web Apps](https://goo.gl/W7VVDo) called [WebJobs](https://goo.gl/xNK1Xz).

The project contains two main files:
- Program.cs: contains the entry point of the application and initialize it
- Function.cs: contains the method triggered at midnight


So let's start completing the *Program.cs*.

Read the connection strings that the program will use to connect to the Azure services like IoT Hub and Storage from the app.config file.
To do this, implementis the following method:


```cs
private static bool VerifyConfiguration()
{
    // Load app connecting string

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
```


The we can complete the *Functions.cs* file.

In the CronJob method we need to call the methods that call with the IoT Hub to enable the devices and to update their states on the corresponding twin:

```cs
// Triggers every day at 00:00:00
public async void CronJob([TimerTrigger("0 0 0 * * *")] TimerInfo timer, TextWriter log)
{
    await _deviceManager.enableAll();
    EnableAllStatus();
}
```


```cs
private async void EnableAllStatus()
{
    var query = _deviceManager.CreateQuery("SELECT * FROM devices WHERE properties.desired.status IN ['disabled', 'warning']", 1000);

    while (query.HasMoreResults)
    {
        var page = await query.GetNextAsTwinAsync();
        foreach (var twin in page)
        {
            var patch = new
            {
                properties = new
                {
                    desired = new
                    {
                        status = "enabled"
                    }
                }
            };

            await _deviceManager.UpdateTwin(twin.DeviceId, JsonConvert.SerializeObject(patch), twin.ETag);
        }
    }
}
```

In the *EnableAllStatus* method we get all the devices which status is *disabled* or *warning* but not the ones that are in the *enable* state.

