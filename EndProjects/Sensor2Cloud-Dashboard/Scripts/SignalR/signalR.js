$(function () {
    // Declare a proxy to reference the hub.
    var sensorHub = $.connection.sensorHub;

    // Create a function that the hub can call to broadcast messages.
    sensorHub.client.addReading = function (reading) {
        // display reading
        //var JsonObj = JSON.parse(reading);

        // NOW we receive a JSON OBJECT directly
        var JsonObj = reading;

        if (JsonObj.mtype != undefined && JsonObj.ts != undefined && JsonObj.name != undefined) {
            addDeviceName(JsonObj.name);
            deviceConnected();

            var x_value_data = new Date(JsonObj.ts);

            if (JsonObj.mtype == "ins" || JsonObj.mtype == "alr") {

                if (JsonObj.temp != undefined) {
                    addTemperatureValue(x_value_data, JsonObj.temp);
                    if (JsonObj.temp >= thresholdValues['tempThreshold']) {
                        addTemperatureAlarmValue(x_value_data, JsonObj.temp, JsonObj.name, "Temperature", JsonObj.message);
                        sendMessageToDevice('alert:temperature');
                    }
                }

                if (JsonObj.hum != undefined) {
                    addHumidityValue(x_value_data, JsonObj.hum);

                    if (JsonObj.hum >= thresholdValues['humThreshold']) {
                        addHumidityAlarmValue(x_value_data, JsonObj.hum, JsonObj.name, "Humidity", JsonObj.message);
                        //sendMessageToDevice('alert:humidity');
                    }
                }

                if (JsonObj.accx != undefined && JsonObj.accy != undefined && JsonObj.accz != undefined) {
                    addAccValue(x_value_data, JsonObj.accx, JsonObj.accy, JsonObj.accz);

                    // ACC X
                    if (JsonObj.accx >= thresholdValues['accXUpperThreshold']) {
                        addAccAlarmValue(x_value_data, JsonObj.accx, JsonObj.name, "Accelerometer X too big", JsonObj.message);
                        //sendMessageToDevice('alert:accx_up');
                    }

                    if (JsonObj.accx <= thresholdValues['accXLowerThreshold']) {
                        addAccAlarmValue(x_value_data, JsonObj.accx, JsonObj.name, "Accelerometer X too low", JsonObj.message);
                        //sendMessageToDevice('alert:accx_down');
                    }

                    // ACC Y
                    if (JsonObj.accy >= thresholdValues['accYUpperThreshold']) {
                        addAccAlarmValue(x_value_data, JsonObj.accy, JsonObj.name, "Accelerometer Y too big", JsonObj.message);
                        //sendMessageToDevice('alert:accy_up');
                    }

                    if (JsonObj.accy <= thresholdValues['accYLowerThreshold']) {
                        addAccAlarmValue(x_value_data, JsonObj.accy, JsonObj.name, "Accelerometer Y too low", JsonObj.message);
                        //sendMessageToDevice('alert:accy_down');
                    }

                    // ACC Z
                    if (JsonObj.accz >= thresholdValues['accZUpperThreshold']) {
                        addAccAlarmValue(x_value_data, JsonObj.accz, JsonObj.name, "Accelerometer Z too big", JsonObj.message);
                        //sendMessageToDevice('alert:accz_up');
                    }

                    if (JsonObj.accz <= thresholdValues['accZLowerThreshold']) {
                        addAccAlarmValue(x_value_data, JsonObj.accz, JsonObj.name, "Accelerometer Z too low", JsonObj.message);
                        //sendMessageToDevice('alert:accz_down');
                    }


                }

                if (JsonObj.gyrx != undefined && JsonObj.gyry != undefined && JsonObj.gyrz != undefined) {
                    addGyrValue(x_value_data, JsonObj.gyrx, JsonObj.gyry, JsonObj.gyrz);

                    // GYR X
                    if (JsonObj.gyrx >= thresholdValues['gyrXUpperThreshold']) {
                        addGyrAlarmValue(x_value_data, JsonObj.gyrx, JsonObj.name, "Gyroscope X too big", JsonObj.message);
                        //sendMessageToDevice('alert:gyrx_up');
                    }

                    if (JsonObj.gyrx <= thresholdValues['gyrXLowerThreshold']) {
                        addGyrAlarmValue(x_value_data, JsonObj.gyrx, JsonObj.name, "Gyroscope X too low", JsonObj.message);
                        //sendMessageToDevice('alert:gyrx_down');
                    }

                    // GYR Y
                    if (JsonObj.gyry >= thresholdValues['gyrYUpperThreshold']) {
                        addGyrAlarmValue(x_value_data, JsonObj.gyry, JsonObj.name, "Gyroscope Y too big", JsonObj.message);
                        //sendMessageToDevice('alert:gyry_up');
                    }

                    if (JsonObj.gyry <= thresholdValues['gyrYLowerThreshold']) {
                        addGyrAlarmValue(x_value_data, JsonObj.gyry, JsonObj.name, "Gyroscope Y too low", JsonObj.message);
                        //sendMessageToDevice('alert:gyry_down');
                    }

                    // GYR Z
                    if (JsonObj.gyrz >= thresholdValues['gyrZUpperThreshold']) {
                        addGyrAlarmValue(x_value_data, JsonObj.gyrz, JsonObj.name, "Gyroscope Z too big", JsonObj.message);
                        //sendMessageToDevice('alert:gyrz_up');
                    }

                    if (JsonObj.gyrz <= thresholdValues['gyrZLowerThreshold']) {
                        addGyrAlarmValue(x_value_data, JsonObj.gyrz, JsonObj.name, "Gyroscope Z too low", JsonObj.message);
                        //sendMessageToDevice('alert:gyrz_down');
                    }

                }
            }
        }
    };


    // Create a function that the hub can call to notify device status change.
    sensorHub.client.deviceStatusChanged = function (newStatus) {
        //console.log("NEW STATUS", newStatus);
        changeDeviceStatus(newStatus);
    }

    // Start the connection.
    $.connection.hub.start().done(function () {
        console.log("hub connected");
    });


});

function registerSignalRClient(macAddress, chartDir, retryCount) {
    saveMAConSessionStorage(macAddress);
    chartDir.fadeIn();
    try {
        if ($.connection.hub.state === $.connection.connectionState["connected"]) {
            $.connection.sensorHub.server.startReading(macAddress);
        }
        else if (!retryCount || retryCount < 5) {
            retryCount = retryCount || 0;
            $.connection.hub.start().done(function () {
                registerSignalRClient(macAddress, chartDir, retryCount + 1);
            });
        }
        else {
            console.log("SIGNALR: Connection Error!");
        }
    } catch (err) {
    }
}

function deregisterSignalRClient(chartDir) {
    // Call the stopReading method on the hub.
    if ($.connection.hub.state === $.connection.connectionState["connected"]) {
        $.connection.sensorHub.server.stopReading(getMACfromSessionStorage());
        $.connection.hub.stop();
    }

    chartDir.hide();
}

function sendMessageToDevice(message) {
    if ($.connection.hub.state === $.connection.connectionState["connected"]) {
        $.connection.sensorHub.server.sendToDevice(getMACfromSessionStorage(), message)
    }
}

function saveMAConSessionStorage(macAddress) {
    if (typeof (Storage) !== "undefined") {
        localStorage.setItem("macAddress", macAddress);
    }
    else {
        // No storage available in this browser
        // Use cookies to save bounding box
    }
}

function removeMACfromSessionStorage() {
    if (typeof (Storage) !== "undefined") {
        return localStorage.removeItem("macAddress");
    }
}

function getMACfromSessionStorage() {

    if (typeof (Storage) !== "undefined") {
        return localStorage.getItem("macAddress");
    }
    else {
        // No storage available in this browser
        // Use cookies to save bounding box
    }
    return null;
}