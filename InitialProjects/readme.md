This folder contains the incomplete version of the Sensors2Cloud web application.

The solution has got 5 projects:
* Sensor2Cloud-Dashboard: [ASP.NET MVC](https://www.asp.net/mvc) web site that shows data coming from the devices in a dashboard and allow user to send command to it
* WebJob-EnableDevices: C# Console Application deployed as [Azure Web Job](https://goo.gl/xNK1Xz) that every day at midnight enable all the devices that are disabled in the IoT Hub
* WebJob-NotifyDevices: C# Console Application deployed as [Azure Web Job](https://goo.gl/xNK1Xz) that change the status of the devices that exceed the maximum number of events they can send each hour
* WebJobs-Shared: C# Class Library containing shared classes between the other web jobs projects
* API: [ASP.NET Web API](https://www.asp.net/web-api) project that expose some REST services that the device can call in order to register itself to the IoT Hub

You must complete the code project by project, in order to have the application working, following the instructions below.


# 1. Sensor2Cloud-Dashboard project

## 1.1 Server side

The .cs files in all the ASP.NET MVC projects like this one, contain the server side logic of the web application and its goal is to provide web pages (.cshtml files) to the user.

The main class of this project is *HomeController.cs*, which contains some public methods called actions. Each action provide to the user browser a specific web page.
For example the *Index* method of the *HomeController* return the web page *Index.cshtml* browsing the url `http://<domain>/Home/Index`.


In the next steps we will complete the code of the *HomeController.cs* class.

1. Load the settings from the configuration file of the project at the beginning of the *Index* method:

        ```cs
        // Load settings from web.config
        model.API_URL = CloudConfigurationManager.GetSetting("API.URL");
        model.BING_SPEECH_API_KEY = CloudConfigurationManager.GetSetting("BING_SPEECH_API_KEY");
        model.LUIS_APP_ID = CloudConfigurationManager.GetSetting("LUIS_APP_ID");
        model.LUIS_SUBSCRIPTION_ID = CloudConfigurationManager.GetSetting("LUIS_SUBSCRIPTION_ID");
        ```
        These are the connection strings of the services that you will create later during the hands on lab.

2. Check if the device is registered and its status that is saved to the IoT Hub [device twin](https://goo.gl/DwvBHf):

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

The server side code of the application is now ready.

## 1.2 Client side

Now we want to add the possibility to set a command by voice, and send it to the device.

To do so we use some [Microsoft Cognitive Services](https://www.microsoft.com/cognitive-services/en-us/apis).

The flow is:
1. Register a command by microphone and send it to the [Bing Speech API](https://www.microsoft.com/cognitive-services/en-us/speech-api) that translate the voice command in a text
2. Send the returned text to [Language Understanding Intelligent Service (LUIS)](https://www.microsoft.com/cognitive-services/en-us/language-understanding-intelligent-service-luis) api, that understand and return the more likely intention of the user
3. Set the intent to the command box in the dashboard web page and send it to the device

The entire flow is managed client side by javascript.

1. Open the *Index.cshtml* page
2. Add the HTML code to the page, that shows the controls to manage the microphone

        ```html
        <div class="register-message-container">
            <button value="Send" id="mic" class="btn btn-circle microphone" onclick="startSpeechToText()">Register</button>
            <div class="register-message-text-container">
                <div id="speech_api_output">Click to register a message for device</div>
                <div id="timer-show"></div>
            </div>
        </div>
        ```
3. Add the javascript function to the script section to retrieve the LUIS credentials of your own instance

        ```javascript
        function getLuisConfig() {
            var appid = '@Model.LUIS_APP_ID';
            var subid = '@Model.LUIS_SUBSCRIPTION_ID';

            if (appid && subid && appid.length > 0 && subid.length > 0) {
                return { appid: appid, subid: subid };
            }

            return null;
        }
        ```

4. Implement the function that manage the microphone and call the Cognitive Services

        ```javascript
        function startSpeechToText() {
            var mode = getMode();
            var luisCfg = getLuisConfig();

            clearText();

            if (luisCfg) {
                // LUIS instance available
                client = Microsoft.CognitiveServices.SpeechRecognition.SpeechRecognitionServiceFactory.createMicrophoneClientWithIntent(
                    getLanguage(),
                    getKey(),
                    luisCfg.appid,
                    luisCfg.subid);
            } else {
                // LUIS instance not available
                client = Microsoft.CognitiveServices.SpeechRecognition.SpeechRecognitionServiceFactory.createMicrophoneClient(
                    mode,
                    getLanguage(),
                    getKey());
            }

            // start microphone registration
            client.startMicAndRecognition();
            $('#mic').addClass('listening');

            // stop microphone registration when timer ends
            timer(registerTimeMs,
                function() {
                    client.endMicAndRecognition();
                    $('#mic').removeClass('listening');
                });

            // show the partial response retrieved by Bing Speech API
            client.onPartialResponseReceived = function (response) {
                setTranscriptText(response);
            }

            // show the final response retrieved by Bing Speech API
            client.onFinalResponseReceived = function (response) {
                console.log("SPEECH_TO_TEXT FINAL RESPONSE", response);
                if(response && response[0] && response[0].transcript)
                    setTranscriptText(response[0].transcript);
            }

            // set the received intent by LUIS to the cloud 2 device message box
            client.onIntentReceived = function (response) {
                var responseJSON = JSON.parse(response);
                console.log("SPEECH_TO_TEXT INTENT RECEIVED", responseJSON);
                if (responseJSON && responseJSON.intents && responseJSON.intents[0] && responseJSON.intents[0].score > 0.5)
                    setIntentText(responseJSON.intents[0].intent);
            };
        }
        ```

The dashboard project is now complete, so you are ready to show the data coming from the devices and send commands to them.

# 2. WebJobs-Shared

This class library project contains a device manager class that interact with the IoT Hub in order to manage the [device registry](https://docs.microsoft.com/it-it/azure/iot-hub/iot-hub-devguide-identity-registry).

The methods implemented by this class can:
- Enable or disable a device or a set of them. The events sent by a disabled device are rejected by the IoT Hub
- Update the device properties saved on its twin

1. Open the *IotHubDeviceManager* class
2. Instantiate a *RegistryManager* class available in the Azure SDK

        ```cs
        private RegistryManager _registryManager;

        public IotHubDeviceManager(string connectionString)
        {
            _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
        }
        ```

3. Implements the methods defined in the *IDeviceManager* interface

        ```cs
        public async Task disable(string mac)
        {
            var device = await _registryManager.GetDeviceAsync(mac);
            device.Status = DeviceStatus.Disabled;
            await _registryManager.UpdateDeviceAsync(device);
        }

        public async Task enable(string mac)
        {
            var device = await _registryManager.GetDeviceAsync(mac);
            device.Status = DeviceStatus.Enabled;
            await _registryManager.UpdateDeviceAsync(device);
        }

        public async Task remove(string mac)
        {
            await _registryManager.RemoveDeviceAsync(mac);
        }

        public async Task enableAll()
        {
            var stats = await _registryManager.GetRegistryStatisticsAsync();
            Console.WriteLine("device count: " + stats.TotalDeviceCount + " enabled: " + stats.EnabledDeviceCount + " disabled: " + stats.DisabledDeviceCount);
            var devices = await _registryManager.GetDevicesAsync((int)stats.TotalDeviceCount * 100); // * 100 because the device list is an approximation 
            devices = devices.Select(d => { d.Status = DeviceStatus.Enabled; return d; });
            await _registryManager.UpdateDevices2Async(devices);
        }

        public async Task disableAll()
        {
            var devices = await _registryManager.GetDevicesAsync(1);
            devices.Select(d => { d.Status = DeviceStatus.Disabled; return d; });
            await _registryManager.UpdateDevices2Async(devices);
        }

        #region Twin

        public IQuery CreateQuery(string query, int size)
        {
            if (size != -1)
                return _registryManager.CreateQuery(query, size);
            else
                return _registryManager.CreateQuery(query);
        }

        public async Task<Twin> GetTwin(string mac)
        {
            var twin = await _registryManager.GetTwinAsync(mac);
            return twin;
        }

        public async Task<Twin> UpdateTwin(string mac, string jsonTwinPatch, string etag)
        {
            var twin = await _registryManager.UpdateTwinAsync(mac, jsonTwinPatch, etag);
            return twin;
        }

        #endregion

        ```

# 3. WebJob-EnableDevices

A device can be disabled if reach the defined daily quota of messages it can send to the IoT Hub.
So we need to check the status of each device registered in the IoT Hub at midnight and enable the previous disabled ones.  

This project contains a small C# program that is triggered by a CRON expression and do exacly what we need.   
In Azure this is possible through a feature of [Azure Web Apps](https://goo.gl/W7VVDo) called [WebJobs](https://goo.gl/xNK1Xz).

The project contains two main files:
- Program.cs: contains the entry point of the application and initialize it
- Function.cs: contains the method triggered at midnight


1. Open the *Program.cs* class.

2. Read the connection strings that the program will use to connect to the Azure services like IoT Hub and Storage from the app.config file. To do this, implementis the following method:

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

3. Open the *Functions.cs* class.

4. By dependency injection get an instance of DeviceManager, that is responsible of all the communication with the IoT Hub.

        ```cs
        private readonly IDeviceManager _deviceManager;

        public Functions(IDeviceManager deviceManager)
        {
            _deviceManager = deviceManager;
        }
        ```

5. In the CronJob method enable all the devices registered in the Iot Hub and update their state:

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

The job that every day call the iot hub to enable all the devices is now implemented.


# 4. WebJob-NotifyDevices

This web job change the status of each device that exceed the hourly quota of events it can send to the IoT Hub.

The device status is implemented as a state machine, so every device can have 3 states:
- **enabled**: it can send events to IoT Hub
- **warning**: set when it exceed the quota for the first time. It can continue to send events to IoT Hub but it is notified by the application to decrease the frequency
- **disabled**: set if the device exceed the quota 2 times consecutively it is disabled and it cannot send events until the next day


The number of events sent from each device is counted by the Stream Analytics connected to the IoT Hub.
Stream Analytics put a new message in a [Service Bus queue](https://azure.microsoft.com/en-us/services/service-bus/), for each device, every hour.
The message contains the number of events of the specified device in the last hour.

This web job is triggered one time for each new message that appear in the queue.

1. Open *Function.cs* class
2. Write the following code in the *SBQueue2IotHubDevice* method, that is triggered by a new service bus message and that check the device's status 

        ```cs
        // Triggered when service bus receive message in a queue 
        public async void SBQueue2IotHubDevice(
            [ServiceBusTrigger("%queueName%")] string message,
            TextWriter log)
        {
            QueueReceivedMessage m = null;

            try
            {
                m = JsonConvert.DeserializeObject<QueueReceivedMessage>(message);
            }
            catch (Exception)
            {
                log.WriteLine("Exception JSONConverter: " + message);
            }

            if (m != null)
            {
                log.WriteLine("SBQueue2IotHubDevice: " + m.MacAddress);

                var twin = await _deviceManager.GetTwin(m.MacAddress);
                if (twin != null)
                {
                    try
                    {
                        var status = twin.Properties.Desired["status"];


                        Console.WriteLine("Status {0}", status, Formatting.Indented);

                        // Max number of messages only for non demo devices

                        if (twin.Tags.Contains("demo") == false)
                        {

                            // State machine

                            if (Convert.ToString(status) == "enabled" && m.MessagesCount > Threshold)
                            {
                                twin = await SetWarning(m.MacAddress, twin.ETag);
                            }
                            else if (Convert.ToString(status) == "warning" && m.MessagesCount > Threshold)
                            {
                                twin = await Disable(m.MacAddress, twin.ETag);
                            }
                            else if (Convert.ToString(status) == "warning" && m.MessagesCount < Threshold)
                            {
                                twin = await Enable(m.MacAddress, twin.ETag);
                            }
                        }

                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        // this device has not status property
                        twin = await SetDefaultStatus(m.MacAddress, twin.ETag);
                    }
                    catch (DeviceNotFoundException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

            }
        }
        ```


Your web job is now complete!


# 5. API

This ASP.NET Web API project, contains some REST services to regiter, remove and get the authentication key (token) of a device.

The *TokenRepository* class manages all the operation with the IoT Hub, that contains the device registry.

1. Open the *TokenRepository* class
2. Define a RegistryManager that will communicate with the IoT Hub and instantiate it in the contructor

        ```cs
        private RegistryManager registryManager;

        public TokensRepository(string iothubConnectionString)
        {
            registryManager = RegistryManager.CreateFromConnectionString(iothubConnectionString);
        }

        ```

3. Implement the method that add a device to the IoT Hub registry and return its personal key

        ```cs
        public async Task<IToken> Add(string deviceId, bool demo)
        {
            try
            {
                Device device = await registryManager.AddDeviceAsync(new Device(deviceId));
                var twin = await Enable(device.Id, demo);

                return new IotHubToken()
                {
                    DeviceID = device.Id,
                    Token = device.Authentication.SymmetricKey.PrimaryKey,
                    Status = twin.Properties.Desired.Contains("status") ? twin.Properties.Desired["status"] : "Unknown"
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        ```

4. Implement the method that return a device token given its id

        ```cs
        public async Task<IToken> Get(string deviceId)
        {

            Device device = await registryManager.GetDeviceAsync(deviceId);

            if (device == null)
                return null;

            var twin = await registryManager.GetTwinAsync(device.Id);

            return new IotHubToken()
            {
                DeviceID = device.Id,
                Token = device.Authentication.SymmetricKey.PrimaryKey,
                Status = twin.Properties.Desired.Contains("status") ? twin.Properties.Desired["status"] : "Unknown"
            };

        }
        ```

5. Implement the method that remove a device from the IoT Hub registry

        ```cs
        public async Task Remove(string deviceId)
        {
            Device device = await registryManager.GetDeviceAsync(deviceId);

            if (device == null)
                throw new DeviceNotFoundException(deviceId);

            await registryManager.RemoveDeviceAsync(device);
        }
        ```

Your API project is now complete!

# 6. Conclusion

The code of the solution is now complete, but in order to be able to build it, we need to set the connection strings of the the Azure services used by this application.

You will see these steps in the [next part](IoTCamp/tree/master/FinalProjects) of the hand on lab.
