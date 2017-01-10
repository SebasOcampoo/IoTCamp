var enableDebugMessages = false;
var DISCONNECTION_INTERVAL = 25000; // ms = 25 secs
var DISCONNECTION_TIMER = null;
var CONNECTED = "connected";
var DISCONNECTED = "disconnected";
var WARNING = "warning";
var ENABLED = "enabled";
var DISABLED = "disabled";



function messageOutput(message, active, error) {

    if (enableDebugMessages) {
    if (error) {
        $('#messages').prepend('<div class="errormessage">' + message + '<div>');
    }
    else {
        $('#messages').prepend('<div>' + message + '<div>');
    }

    
        if (active == true) {
            $('#messages-container').style.display = 'block';
        }
        else {
            $('#messages-container').style.display = 'none';
        }
    }
    

}

function saveInSessionStorage(key, val) {
    if (typeof (Storage) !== "undefined") { // check if browser support storage
        window.sessionStorage.setItem(key, val);
    }
}
function getFromSessionStorage(key) {
    if (typeof (Storage) !== "undefined") { // check if browser support storage
        return window.sessionStorage.getItem(key);
    }
    return null;
}

var SESSION_STORAGE_MAC_POSITION_KEY = "MAC_POSITION"

function saveLocation(macAddress) {
    if (getFromSessionStorage(SESSION_STORAGE_MAC_POSITION_KEY) != macAddress) { // save position only 1 time per user's session
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition((position) => { savePosition(macAddress, position) }, (error) => { handleError(macAddress, error) });
        } else {
            handleError({ code: "NOT_SUPPORTED" });
        }
    }
}

function savePosition(macAddress, position) {
    var userData = {
        Longitude: position.coords.longitude,
        Latitude: position.coords.latitude,
        Timestamp: new Date().toISOString(),
        DeviceMacAddress: macAddress
    };
    //console.log("saving", userData);
    $.post(savePositionURL, userData).done(function (data) {
        saveInSessionStorage(SESSION_STORAGE_MAC_POSITION_KEY, macAddress);
        //console.log( "Data Loaded: ",  data );
    });
}
function handleError(macAddress, error) {
    switch (error.code) {
        case error.PERMISSION_DENIED:
        case error.POSITION_UNAVAILABLE:
        case error.TIMEOUT:
        case error.UNKNOWN_ERROR:
        case "NOT_SUPPORTED":
            var userData = {
                Longitude: 0,
                Latitude: 0,
                Timestamp: new Date().toISOString(),
                DeviceMacAddress: macAddress
            };

            //console.log("error saving", userData);

            $.post(savePositionURL, userData).done(function (data) {
                //console.log("Data Loaded: ", data);
            });

            break;
    }
}

function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}

function changeDeviceConnectionStatus(status) {
    var bullet = $('.device-connection .bullet');
    bullet.removeClass();
    bullet.addClass("bullet " + status);
    $('.device-connection .bullet-text').text(capitalizeFirstLetter(status));
}

function changeDeviceStatus(status) {
    var bullet = $('.device-status .bullet');
    bullet.removeClass();
    bullet.addClass("bullet " + status);
    $('.device-status .bullet-text').text(capitalizeFirstLetter(status));
}

function deviceConnected() {
    changeDeviceConnectionStatus(CONNECTED)

    if (DISCONNECTION_TIMER) {
        clearTimeout(DISCONNECTION_TIMER)
    }

    DISCONNECTION_TIMER = setTimeout(function () {
        changeDeviceConnectionStatus(DISCONNECTED)
    }, DISCONNECTION_INTERVAL);
}