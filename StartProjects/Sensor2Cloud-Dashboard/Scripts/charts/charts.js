var chartTemperature = null
var dataPointsTemperature = [];
var dataPointsTemperatureAverage = [];
var dataPointsTemperatureAlarm = [];
var dataPointsTemperatureAlarmLine = [];
var dataTempLength = 80;

var chartHumidity = null
var dataPointsHumidity = [];
var dataPointsHumidityAverage = [];
var dataPointsHumidityAlarm = [];
var dataPointsHumidityAlarmLine = [];
var dataHumLength = 80;


var chartAccelerometer = null
var dataPoints_XAccelerometer = [];
var dataPoints_YAccelerometer = [];
var dataPoints_ZAccelerometer = [];
var dataPointsAccelerometerAlarm = [];
var dataPointsAccelerometerAlarmLine = [];
var dataAccLength = 80;

var chartGyroscope = null
var dataPoints_XGyroscope = [];
var dataPoints_YGyroscope = [];
var dataPoints_ZGyroscope = [];
var dataPointsGyroscopeAlarm = [];
var dataPointsGyroscopeAlarmLine = [];
var dataGyrLength = 80;

var AvgCamp = 20;

var MaxnumAlarmTableLine = 25;
var DeviceName = "";

var table = null;
var enableTemp = false;
var enableHum = false;
var enableAcc = false;
var enableGyr = false;


function addDeviceName(name) {
    DeviceName = name;
    
    if (name != "") {
        $('#containerName').html(name);
    }
}

function emptyTemperature() {
    if (chartTemperature != null) {

        while (dataPointsTemperature.length) {
            dataPointsTemperature.pop();
        }

        while (dataPointsTemperatureAverage.length) {
            dataPointsTemperatureAverage.pop();
        }

        while (dataPointsTemperatureAlarm.length) {
            dataPointsTemperatureAlarm.pop();
        }

        while (dataPointsTemperatureAlarmLine.length) {
            dataPointsTemperatureAlarmLine.pop();
        }

        chartTemperature.render();

    }

    while (document.getElementById("table_Alarm_temperature").rows.length > 2)
        document.getElementById("table_Alarm_temperature").deleteRow(document.getElementById("table_Alarm_temperature").rows.length - 1);
}

function emptyHumidity() {
    if (chartHumidity != null) {

        while (dataPointsHumidity.length) {
            dataPointsHumidity.pop();
        }

        while (dataPointsHumidityAverage.length) {
            dataPointsHumidityAverage.pop();
        }

        while (dataPointsHumidityAlarm.length) {
            dataPointsHumidityAlarm.pop();
        }

        while (dataPointsHumidityAlarmLine.length) {
            dataPointsHumidityAlarmLine.pop();
        }

        chartHumidity.render();
    }

    while (document.getElementById("table_Alarm_humidity").rows.length > 2)
        document.getElementById("table_Alarm_humidity").deleteRow(document.getElementById("table_Alarm_humidity").rows.length - 1);
}


function emptyAccelerometer() {

    if (chartAccelerometer != null) {


        while (dataPoints_XAccelerometer.length) {
            dataPoints_XAccelerometer.pop();
        }

        while (dataPoints_YAccelerometer.length) {
            dataPoints_YAccelerometer.pop();
        }

        while (dataPoints_ZAccelerometer.length) {
            dataPoints_ZAccelerometer.pop();
        }

        chartAccelerometer.render();

    }
}


function emptyGyroscope() {
    if (chartGyroscope != null) {

        while (dataPoints_XGyroscope.length) {
            dataPoints_XGyroscope.pop();
            chartGyroscope.render();
        }

        while (dataPoints_YGyroscope.length) {
            dataPoints_YGyroscope.pop();
            chartGyroscope.render();
        }

        while (dataPoints_ZGyroscope.length) {
            dataPoints_ZGyroscope.pop();
            chartGyroscope.render();
        }
    }
}

function showTemperatureChart(enable) {

    if (enable) {
        $('#chartTemperatureContainer').show();
        $('#tableAlarmsContainerTemperature').show();
        $('#noChartTemperatureContainer').hide();
        $('#temperatureThresholdContainer').show();
        var tab = $('a[href="#temperature"]');
        if (!tab.is(":visible")) {
            if ($('a[data-toggle="tab"]:visible').length < 1)
                tab.tab('show')
            tab.show();
            tab.css('display', 'inline-block');
        }
    }
    else {
        $('#chartTemperatureContainer').hide();
        $('#tableAlarmsContainerTemperature').hide();
        $('#noChartTemperatureContainer').show();
        $('a[href="#temperature"]').hide();
        $('#temperatureThresholdContainer').hide();

        emptyTemperature();
    }

}

function showHumidityChart(enable) {

    if (enable) {
        $('#chartHumidityContainer').show();
        $('#tableAlarmsContainerHumidity').show();
        $('#noChartHumidityContainer').hide();
        $('#humidityThresholdContainer').show();
        var tab = $('a[href="#humidity"]');
        if (!tab.is(":visible")) {
            if ($('a[data-toggle="tab"]:visible').length < 1)
                tab.tab('show')
            tab.show();
            tab.css('display', 'inline-block');
        }
    }
    else {
        $('#chartHumidityContainer').hide();
        $('#tableAlarmsContainerHumidity').hide();
        $('#noChartHumidityContainer').show();
        $('a[href="#humidity"]').hide();
        $('#humidityThresholdContainer').hide();

        emptyHumidity();
    }

}

function showAccelerometerChart(enable) {

    if (enable) {
        $('#chartAccelerometerContainer').show();
        $('#tableAlarmsContainerAccelerometer').show();
        $('#noChartAccelerometerContainer').hide();
        $('#accelorometerThresholdContainer').show();
        var tab = $('a[href="#accelerometer"]');
        if (!tab.is(":visible")) {
            if ($('a[data-toggle="tab"]:visible').length < 1)
                tab.tab('show')
            tab.show();
            tab.css('display', 'inline-block');
        }
    }
    else {
        $('#chartAccelerometerContainer').hide();
        $('#tableAlarmsContainerAccelerometer').hide();
        $('#noChartAccelerometerContainer').show();
        $('a[href="#accelerometer"]').hide();
        $('#accelorometerThresholdContainer').hide();

        emptyAccelerometer();
    }

}

function showGyroscopeChart(enable) {

    if (enable) {
        $('#chartGyroscopeContainer').show();
        $('#tableAlarmsContainerGyroscope').show();
        $('#noChartGyroscopeContainer').hide();
        $('#gyrThresholdContainer').show();
        var tab = $('a[href="#gyroscope"]');
        if (!tab.is(":visible")) {
            if ($('a[data-toggle="tab"]:visible').length < 1)
                tab.tab('show')
            tab.show();
            tab.css('display', 'inline-block');
        }
    }
    else {
        $('#chartGyroscopeContainer').hide();
        $('#tableAlarmsContainerGyroscope').hide();
        $('#noChartGyroscopeContainer').show();
        $('a[href="#gyroscope"]').hide();
        $('#gyrThresholdContainer').hide();

        emptyGyroscope();
    }

}

function HideCharts() {
    showHumidityChart(false);
    showTemperatureChart(false);
    showAccelerometerChart(false);
    showGyroscopeChart(false);
}


function createChartTemperature() {

    showTemperatureChart(true);

    chartTemperature = new CanvasJS.Chart("chartTemperatureContainer", {
        dataPointMaxWidth: 20,
        backgroundColor: "transparent",
        interactivityEnabled: true,
        xValueType: "dateTime",
        axisY: {
            title: "Temperature (\u00B0C)",
            includeZero: false,
            suffix: " \u00B0"
        },
        axisX: {
            stripLines: dataPointsTemperatureAlarmLine,
        },
        legend: {
            fontFamily: "Helvetica",
            itemclick: function (e) {

                if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
                    e.dataSeries.visible = false;
                } else {
                    e.dataSeries.visible = true;
                }

                e.chart.render();
            }
        },
        data: [
        {
            type: "spline",
            dataPoints: dataPointsTemperature,
            showInLegend: "true",
            legendText: "Temperature",
        },
        {
            type: "scatter",
            dataPoints: dataPointsTemperatureAlarm,
            markerSize: 15,
            markerColor: "red",
            markerType: "circle",
            showInLegend: "true",
            legendText: "Alarm",
        }


        ]


    });
    chartTemperature.render();
}


function createChartHumidity() {

    showHumidityChart(true);

    chartHumidity = new CanvasJS.Chart("chartHumidityContainer", {
        dataPointMaxWidth: 20,
        backgroundColor: "transparent",

        interactivityEnabled: true,
        xValueType: "dateTime",
        axisY: {
            title: "Humidity (%)",
            includeZero: false,
            suffix: " %"
        },
        axisX: {
            stripLines: dataPointsHumidityAlarmLine,
        },
        legend: {
            fontFamily: "Helvetica",
            itemclick: function (e) {

                if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
                    e.dataSeries.visible = false;
                } else {
                    e.dataSeries.visible = true;
                }

                e.chart.render();
            }
        },
        data: [
        {
            type: "spline",
            dataPoints: dataPointsHumidity,
            showInLegend: "true",
            legendText: "Humidity",
        },
        {
            type: "scatter",
            dataPoints: dataPointsHumidityAlarm,
            markerSize: 15,
            markerColor: "red",
            markerType: "circle",
            showInLegend: "true",
            legendText: "Alarm",
        }
        ]
    });
    chartHumidity.render();
}

function addTemperatureValue(x_val, y_val) {

    if (y_val != 0)
        enableTemp = true;

    if (!enableTemp)
        return;

    showTemperatureChart(true);

    if (chartTemperature == null) {
        createChartTemperature()
    }

    if (chartTemperature != null) {
        dataPointsTemperature.push({ x: x_val, y: y_val });

        if (dataPointsTemperature.length > dataTempLength) {
            dataPointsTemperature.shift();
        }


        if (dataPointsTemperatureAverage.length && dataPointsTemperature.length &&
            dataPointsTemperatureAverage[0].x < dataPointsTemperature[0].x) {
            dataPointsTemperatureAverage.shift();
        }

        if (dataPointsTemperatureAlarm.length && dataPointsTemperature.length &&
            dataPointsTemperatureAlarm[0].x < dataPointsTemperature[0].x) {
            dataPointsTemperatureAlarm.shift();
        }

        chartTemperature.render();
    }

}

function addTemperatureAvgValue(x_val, y_val) {

    if (y_val != 0)
        enableTemp = true;

    if (!enableTemp)
        return;

    if (dataPointsTemperature.length &&
            x_val < dataPointsTemperature[0].x) {
        return;
    }

    showTemperatureChart(true);

    if (chartTemperature == null) {
        createChartTemperature()
    }

    if (chartTemperature != null) {
        dataPointsTemperatureAverage.push({ x: x_val, y: y_val });

        chartTemperature.render();
    }
}

function addAlarmTableLineTemperature(time, device_name, alert_type, message) {
    showTemperatureChart(true);

    var tableRef = document.getElementById('table_Alarm_temperature').getElementsByTagName('tbody')[0]
    var line = tableRef.insertRow(1)
     
    line.className = "alarmLine";

    line.appendChild(document.createElement('td'));
    line.appendChild(document.createElement('td'));
    line.appendChild(document.createElement('td'));
    line.appendChild(document.createElement('td'));

    line.cells[0].appendChild(document.createTextNode(time));
    line.cells[1].appendChild(document.createTextNode(device_name));
    line.cells[2].appendChild(document.createTextNode(alert_type));
    line.cells[3].appendChild(document.createTextNode(message));



    while (document.getElementById("table_Alarm_temperature").rows.length > (MaxnumAlarmTableLine + 1))
        document.getElementById("table_Alarm_temperature").deleteRow(tableRef.rows.length - 1);

}

function addAlarmTableLineAcceleromer(time, device_name, alert_type, message) {
    showAccelerometerChart(true);

    var tableRef = document.getElementById('table_Alarm_Accelerometer').getElementsByTagName('tbody')[0]
    var line = tableRef.insertRow(1)

    line.className = "alarmLine";

    line.appendChild(document.createElement('td'));
    line.appendChild(document.createElement('td'));
    line.appendChild(document.createElement('td'));
    line.appendChild(document.createElement('td'));

    line.cells[0].appendChild(document.createTextNode(time));
    line.cells[1].appendChild(document.createTextNode(device_name));
    line.cells[2].appendChild(document.createTextNode(alert_type));
    line.cells[3].appendChild(document.createTextNode(message));



    while (document.getElementById("table_Alarm_Accelerometer").rows.length > (MaxnumAlarmTableLine + 1))
        document.getElementById("table_Alarm_Accelerometer").deleteRow(tableRef.rows.length - 1);

}

function addAlarmTableLineGyroscope(time, device_name, alert_type, message) {
    showGyroscopeChart(true);

    var tableRef = document.getElementById('table_Alarm_Gyroscope').getElementsByTagName('tbody')[0]
    var line = tableRef.insertRow(1)

    line.className = "alarmLine";

    line.appendChild(document.createElement('td'));
    line.appendChild(document.createElement('td'));
    line.appendChild(document.createElement('td'));
    line.appendChild(document.createElement('td'));

    line.cells[0].appendChild(document.createTextNode(time));
    line.cells[1].appendChild(document.createTextNode(device_name));
    line.cells[2].appendChild(document.createTextNode(alert_type));
    line.cells[3].appendChild(document.createTextNode(message));



    while (document.getElementById("table_Alarm_Gyroscope").rows.length > (MaxnumAlarmTableLine + 1))
        document.getElementById("table_Alarm_Gyroscope").deleteRow(tableRef.rows.length - 1);

}


function addTemperatureAlarmValue(x_val, y_val, device_name, alert_type, message) {

    if (dataPointsTemperature.length &&
            x_val < dataPointsTemperature[0].x) {
        return;
    }
    
    showTemperatureChart(true);

    if (chartTemperature == null) {
        createChartTemperature()
    }

    if (!dataPointsTemperatureAlarm.length) {
        $('#tableAlarmsContainerTemperature').show();
    }

    if (chartTemperature != null) {
        dataPointsTemperatureAlarm.push({ x: x_val, y: y_val });
        dataPointsTemperatureAlarmLine.push({ value: x_val });
        chartTemperature.render();
        addAlarmTableLineTemperature(x_val, device_name, alert_type, message);
    }
}


function addHumidityValue(x_val, y_val) {

    if (y_val != 0)
        enableHum = true;

    if (!enableHum)
        return;

    showHumidityChart(true);

    if (chartHumidity == null) {
        createChartHumidity()
    }

    if (chartHumidity != null) {
        dataPointsHumidity.push({ x: x_val, y: y_val });

        if (dataPointsHumidity.length > dataHumLength) {
            dataPointsHumidity.shift();
        }

        if (dataPointsHumidityAverage.length && dataPointsHumidity.length &&
            dataPointsHumidityAverage[0].x < dataPointsHumidity[0].x) {
            dataPointsHumidityAverage.shift();
        }

        if (dataPointsHumidityAlarm.length && dataPointsHumidity.length &&
            dataPointsHumidityAlarm[0].x < dataPointsHumidity[0].x) {
            dataPointsHumidityAlarm.shift();
        }
        
        chartHumidity.render();
    }

}

function addHumidityAvgValue(x_val, y_val) {

    if (dataPointsHumidity.length && x_val < dataPointsHumidity[0].x) {
        return;
    }

    showHumidityChart(true);

    if (chartHumidity == null) {
        createChartHumidity()
    }

    if (chartHumidity != null) {
        dataPointsHumidityAverage.push({ x: x_val, y: y_val });

        chartHumidity.render();
    }
}


function addAlarmTableLineHumidity(time, device_name, alert_type, message) {

    showHumidityChart(true);

    var tableRef = document.getElementById('table_Alarm_humidity').getElementsByTagName('tbody')[0]
    var line = tableRef.insertRow(1)

    line.className = "alarmLine";

    line.appendChild(document.createElement('td'));
    line.appendChild(document.createElement('td'));
    line.appendChild(document.createElement('td'));
    line.appendChild(document.createElement('td'));

    line.cells[0].appendChild(document.createTextNode(time));
    line.cells[1].appendChild(document.createTextNode(device_name));
    line.cells[2].appendChild(document.createTextNode(alert_type));
    line.cells[3].appendChild(document.createTextNode(message));



    while (document.getElementById("table_Alarm_humidity").rows.length > (MaxnumAlarmTableLine + 1))
        document.getElementById("table_Alarm_humidity").deleteRow(tableRef.rows.length - 1);

}

function addHumidityAlarmValue(x_val, y_val, device_name, alert_type, message) {

    if (dataPointsHumidity.length &&
            x_val < dataPointsHumidity[0].x) {
        return;
    }

    showHumidityChart(true);

    if (chartHumidity == null) {
        createChartHumidity()
    }

    if (!dataPointsHumidityAlarm.length) {
        $('#tableAlarmsContainerHumidity').show();
    }

    if (chartHumidity != null) {
        dataPointsHumidityAlarm.push({ x: x_val, y: y_val });
        dataPointsHumidityAlarmLine.push({ value: x_val });
        chartHumidity.render();
        addAlarmTableLineHumidity(x_val, device_name, alert_type, message);
    }

    if (dataPointsHumidityAlarm.length && dataPointsHumidity.length &&
            dataPointsHumidityAlarm[0].x < dataPointsHumidity[0].x) {
        dataPointsHumidityAlarm.shift();
    }
}

function createChartAccelerometer() {

    showAccelerometerChart(true);

    chartAccelerometer = new CanvasJS.Chart("chartAccelerometerContainer", {
        dataPointMaxWidth: 20,
        backgroundColor: "transparent",
        interactivityEnabled: true,
        xValueType: "dateTime",
        axisY: {
            title: "Acceleration",
            includeZero: true,
            suffix: " mm/s"
        },
        axisX: {
            stripLines: dataPointsAccelerometerAlarmLine,
        },
        legend: {
            fontFamily: "Helvetica",
            itemclick: function (e) {

                if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
                    e.dataSeries.visible = false;
                } else {
                    e.dataSeries.visible = true;
                }

                e.chart.render();
            }
        },
        data: [
        {
            type: "spline",
            dataPoints: dataPoints_XAccelerometer,
            showInLegend: "true",
            legendText: "X Acc.",
        },
        {
            type: "spline",
            dataPoints: dataPoints_YAccelerometer,
            showInLegend: "true",
            legendText: "Y Acc.",
        },
        {
            type: "spline",
            dataPoints: dataPoints_ZAccelerometer,
            showInLegend: "true",
            legendText: "Z Acc.",
        },
        {
            type: "scatter",
            dataPoints: dataPointsAccelerometerAlarm,
            markerSize: 15,
            markerColor: "red",
            markerType: "circle",
            showInLegend: "true",
            legendText: "Alarm",
        }
        ]


    });
    chartAccelerometer.render();
}



function addAccValue(x_val, y_val_x, y_val_y, y_val_z) {

    if (y_val_x != 0 || y_val_y != 0 || y_val_z != 0)
        enableAcc = true;

    if (!enableAcc)
        return;


    showAccelerometerChart(true);

    if (chartAccelerometer == null) {
        createChartAccelerometer()
    }

    if (chartAccelerometer != null) {
        dataPoints_XAccelerometer.push({ x: x_val, y: y_val_x });
        dataPoints_YAccelerometer.push({ x: x_val, y: y_val_y });
        dataPoints_ZAccelerometer.push({ x: x_val, y: y_val_z });

        if (dataPoints_XAccelerometer.length > dataAccLength / 2) {
            dataPoints_XAccelerometer.shift();
        }

        if (dataPoints_YAccelerometer.length > dataAccLength / 2) {
            dataPoints_YAccelerometer.shift();
        }

        if (dataPoints_ZAccelerometer.length > dataAccLength / 2) {
            dataPoints_ZAccelerometer.shift();
        }

        if (dataPointsAccelerometerAlarm.length && dataPoints_XAccelerometer.length)
            while(dataPointsAccelerometerAlarm[0].x < dataPoints_XAccelerometer[0].x) {
                dataPointsAccelerometerAlarm.shift();
        }

        chartAccelerometer.render();
    }
}

function addAccAlarmValue(x_val, y_val, device_name, alert_type, message) {
    if (dataPoints_XAccelerometer.length &&
            x_val < dataPoints_XAccelerometer[0].x) {
        return;
    }

    showAccelerometerChart(true);

    if (chartAccelerometer == null) {
        createChartAccelerometer()
    }

    if (!dataPointsAccelerometerAlarm.length) {
        $('#tableAlarmsContainerAccelerometer').show();
    }

    if (chartAccelerometer != null) {
        dataPointsAccelerometerAlarm.push({ x: x_val, y: y_val });
        dataPointsAccelerometerAlarmLine.push({ value: x_val });
        chartAccelerometer.render();
        addAlarmTableLineAcceleromer(x_val, device_name, alert_type, message);
    }
}


function createChartGyroscope() {

    showGyroscopeChart(true);

    chartGyroscope = new CanvasJS.Chart("chartGyroscopeContainer", {
        dataPointMaxWidth: 20,
        backgroundColor: "transparent",
        interactivityEnabled: true,
        xValueType: "dateTime",
        axisY: {
            title: "Angular acceleration",
            includeZero: true,
            suffix: " \u00B0/s"
        },
        axisX: {
            stripLines: dataPointsGyroscopeAlarmLine,
        },
        legend: {
            fontFamily: "Helvetica",
            itemclick: function (e) {

                if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
                    e.dataSeries.visible = false;
                } else {
                    e.dataSeries.visible = true;
                }

                e.chart.render();
            }
        },
        data: [
        {
            type: "spline",
            dataPoints: dataPoints_XGyroscope,
            showInLegend: "true",
            legendText: "X Gyr.",
        },
        {
            type: "spline",
            dataPoints: dataPoints_YGyroscope,
            showInLegend: "true",
            legendText: "Y Gyr.",
        },
        {
            type: "spline",
            dataPoints: dataPoints_ZGyroscope,
            showInLegend: "true",
            legendText: "Z Gyr.",
        },
        {
            type: "scatter",
            dataPoints: dataPointsGyroscopeAlarm,
            markerSize: 15,
            markerColor: "red",
            markerType: "circle",
            showInLegend: "true",
            legendText: "Alarm",
        }
        ]


    });
    chartGyroscope.render();
}



function addGyrValue(x_val, y_val_x, y_val_y, y_val_z) {

    if (y_val_x != 0 || y_val_y != 0 || y_val_z != 0)
        enableGyr = true;

    if (!enableGyr)
        return;

    showGyroscopeChart(true);

    if (chartGyroscope == null) {
        createChartGyroscope()
    }

    if (chartGyroscope != null) {
        dataPoints_XGyroscope.push({ x: x_val, y: y_val_x });
        dataPoints_YGyroscope.push({ x: x_val, y: y_val_y });
        dataPoints_ZGyroscope.push({ x: x_val, y: y_val_z });

        if (dataPoints_XGyroscope.length > dataGyrLength / 2) {
            dataPoints_XGyroscope.shift();
        }

        if (dataPoints_YGyroscope.length > dataGyrLength / 2) {
            dataPoints_YGyroscope.shift();
        }

        if (dataPoints_ZGyroscope.length > dataGyrLength / 2) {
            dataPoints_ZGyroscope.shift();
        }

        if (dataPointsGyroscopeAlarm.length && dataPoints_XGyroscope.length)
            while(dataPointsGyroscopeAlarm[0].x < dataPoints_XGyroscope[0].x) {
                dataPointsGyroscopeAlarm.shift();
        }

        chartGyroscope.render();
    }
}

function addGyrAlarmValue(x_val, y_val, device_name, alert_type, message) {

    if (dataPoints_XGyroscope.length &&
            x_val < dataPoints_XGyroscope[0].x) {
        return;
    }

    showGyroscopeChart(true);

    if (chartGyroscope == null) {
        createChartGyroscope()
    }

    if (!dataPointsGyroscopeAlarm.length) {
        $('#tableAlarmsContainerGyroscope').show();
    }

    if (chartGyroscope != null) {
        dataPointsGyroscopeAlarm.push({ x: x_val, y: y_val });
        dataPointsGyroscopeAlarmLine.push({ value: x_val });
        chartGyroscope.render();
        addAlarmTableLineGyroscope(x_val, device_name, alert_type, message);
    }
}

function graphRender(tab) {
    switch (tab) {
        case "#temperature":
            if(chartTemperature)
                chartTemperature.render();
            break;
        case "#humidity":
            if(chartHumidity)
                chartHumidity.render();
            break;
        case "#accelerometer":
            if (chartAccelerometer)
                chartAccelerometer.render();
            break;
        case "#gyroscope":
            if (chartGyroscope)
                chartGyroscope.render();
            break;
    }
}