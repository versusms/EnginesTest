/***********************************************
               GLOBAL SETTINGS
***********************************************/
var CTitleSize = 20;
var CTitleColor = "#dddddd";
var CATitleSize = 16;
var CALabelSize = 10;
var CBGColor = "#3d3d3d";
/**********************************************/

// Измерения
Application.Measurements = {
    Torque: [],
    RPM: [],
    FuelConsumption: [],
    TCoolant: [],
    TOil: [],
    TFuel: [],
    TExhaustGas: [],
    POil: [],
    PExhaustGas: [],
    PowerHP: [],
    PowerKWh: []
};
Application.Measurements.Reset = function () {
    $.each(Application.Measurements, function (i, v) {
        if (Array.isArray(Application.Measurements[i])) {
            while (Application.Measurements[i].length) {
                Application.Measurements[i].pop();
            }
        }
    });
}
Application.Measurements.Push = function (number, data) {
    Application.Measurements.Torque.push({ x: number, y: data.Torque });
    Application.Measurements.RPM.push({ x: number, y: data.RPM });
    Application.Measurements.FuelConsumption.push({ x: data.RPM, y: data.FuelConsumption });
    Application.Measurements.PowerHP.push({ x: data.RPM, y: data.PowerHP });
    Application.Measurements.PowerKWh.push({ x: data.RPM, y: data.PowerKWh });
    Application.Measurements.TCoolant.push({ x: number, y: data.TCoolant });
    Application.Measurements.TOil.push({ x: number, y: data.TOil });    
    Application.Measurements.TFuel.push({ x: number, y: data.TFuel });
    Application.Measurements.TExhaustGas.push({ x: number, y: data.TExhaustGas });
    Application.Measurements.POil.push({ x: number, y: data.POil });
    Application.Measurements.PExhaustGas.push({ x: number, y: data.PExhaustGas });
}
Application.Measurements.Load = function (testId) {
    Application.Measurements.Reset();
    $.get("/api/measurements/", { TestId: testId }, function (response) {
        if (response.Success) {
            $.each(response.DataSet, function (i, v) {
                Application.Measurements.Push(i, v);
            });
            Application.Charts.Render();
        }
        else {
            Application.ShowError(response.Message);
        }
    });
}

// Графики
Application.Charts = {};
Application.Charts.Init = function () {
    Application.Measurements.Reset();
    Application.Charts.RPMTorque = new CanvasJS.Chart('chart-RPMTorque', Application.Charts.Configs.RPMTorque);
    Application.Charts.RPMFuelConsumption = new CanvasJS.Chart('chart-RPMFuelConsumption', Application.Charts.Configs.RPMFuelConsumption);
    Application.Charts.RPMPowerHP = new CanvasJS.Chart('chart-RPMPowerHP', Application.Charts.Configs.RPMPowerHP);
    Application.Charts.RPMPowerKWh = new CanvasJS.Chart('chart-RPMPowerKWh', Application.Charts.Configs.RPMPowerKWh);
    Application.Charts.TemperatureCO = new CanvasJS.Chart('chart-TemperatureCO', Application.Charts.Configs.TemperatureCO);
    Application.Charts.TemperatureFE = new CanvasJS.Chart('chart-TemperatureFE', Application.Charts.Configs.TemperatureFE);
    Application.Charts.Pressure = new CanvasJS.Chart('chart-Pressure', Application.Charts.Configs.Pressure);
}
Application.Charts.Render = function () {
    try{
        Application.Charts.RPMTorque.render();
        Application.Charts.RPMFuelConsumption.render();
        Application.Charts.RPMPowerHP.render();
        Application.Charts.RPMPowerKWh.render();
        Application.Charts.TemperatureCO.render();
        Application.Charts.TemperatureFE.render();
        Application.Charts.Pressure.render();
    } catch (e) { }
}

Application.Charts.Configs = {}
Application.Charts.Configs.RPMTorque = {
    animationEnabled: true,
    title: { text: 'RPM / Torque', fontColor: CTitleColor, fontSize: CTitleSize },
    backgroundColor: CBGColor,
    axisX: {
        title: "Time, s",
        titleFontColor: CTitleColor,        
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        interval: 5,
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        maximum: 30
    },
    axisY: {
        title: "RPM",
        titleFontColor: "#de7d36",
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        //interval: 2000,
        lineColor: "#de7d36",
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        //maximum: 9500
    },
    axisY2: {
        title: "Torque, NM",
        titleFontColor: "#C24642",        
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        //interval: 200,
        lineColor: "#C24642",
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        //maximum: 1200
    },
    toolTip: { shared: true },
    data: [
        {
            type: "spline",
            name: "RPM",
            color: "#de7d36",
            dataPoints: Application.Measurements.RPM
        }, {
            type: "spline",
            name: "Torque",
            color: "#C24642",
            axisYType: "secondary",
            dataPoints: Application.Measurements.Torque
        }
    ]
};
Application.Charts.Configs.RPMPowerHP = {
    animationEnabled: true,
    title: { text: 'RPM / Power', fontColor: CTitleColor, fontSize: CTitleSize },
    backgroundColor: CBGColor,
    axisX: {
        title: "RPM",
        titleFontColor: CTitleColor,        
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        interval: 2000,
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        //maximum: 9500
    },
    axisY: {
        title: "Power, HP",
        titleFontColor: "#c8b631",
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        //interval: 150,
        lineColor: "#c8b631",
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        //maximum: 1500
    },
    data: [
        {
            type: "spline",
            color: "#c8b631",
            dataPoints: Application.Measurements.PowerHP
        }
    ]
};
Application.Charts.Configs.RPMPowerKWh = {
    animationEnabled: true,
    title: { text: 'RPM / Power', fontColor: CTitleColor, fontSize: CTitleSize },
    backgroundColor: CBGColor,
    axisX: {
        title: "RPM",
        titleFontColor: CTitleColor,        
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        interval: 2000,
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        //maximum: 9500
    },
    axisY: {
        title: "Power, KWh",
        titleFontColor: "#c8b631",
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        //interval: 150,
        lineColor: "#c8b631",
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        //maximum: 1500
    },
    data: [
        {
            type: "spline",
            color: "#c8b631",
            dataPoints: Application.Measurements.PowerKWh
        }
    ]
};
Application.Charts.Configs.RPMFuelConsumption = {
    animationEnabled: true,
    title: { text: 'RPM / Fuel Consumption', fontColor: CTitleColor, fontSize: CTitleSize },
    backgroundColor: CBGColor,    
    axisX: {
        title: "RPM",
        titleFontColor: CTitleColor,        
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        interval: 2000,
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        //maximum: 9500
    },
    axisY: {
        title: "Fuel Consumption, L/100km",
        titleFontColor: "#7f6084",
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        //interval: 5,
        lineColor: "#7f6084",
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        //maximum: 50
    },
    data: [
        {
            type: "spline",
            color: "#7f6084",
            dataPoints: Application.Measurements.FuelConsumption
        }
    ]
};
Application.Charts.Configs.TemperatureCO = {
    animationEnabled: true,
    title: { text: 'Temperature', fontColor: CTitleColor, fontSize: CTitleSize },
    backgroundColor: CBGColor,
    axisX: {
        title: "Time, s",
        titleFontColor: CTitleColor,        
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        interval: 5,
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        maximum: 30
    },
    axisY: {
        title: "Temperature, C",
        titleFontColor: CTitleColor,
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        //interval: 20,
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 50,
        //maximum: 130
    },
    axisY2: {
        title: "RPM",
        titleFontColor: "#de7d36",
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        //interval: 2000,
        lineColor: "#de7d36",
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        //maximum: 9500
    },
    legend: { horizontalAlign: "center", fontColor: CTitleColor },
    toolTip: { shared: true },
    data: [
        {
            type: "spline",
            showInLegend: true,
            name: "Coolant",            
            color: "#6dbceb",
            dataPoints: Application.Measurements.TCoolant
        },
        {
            type: "spline",
            showInLegend: true,
            name: "Oil",
            color: "#86b402",
            dataPoints: Application.Measurements.TOil
        },
        {
            type: "spline",
            name: "RPM",
            color: "#de7d36",
            axisYType: "secondary",
            dataPoints: Application.Measurements.RPM
        }
    ]
};
Application.Charts.Configs.TemperatureFE = {
    animationEnabled: true,
    title: { text: 'Temperature', fontColor: CTitleColor, fontSize: CTitleSize },
    backgroundColor: CBGColor,
    axisX: {
        title: "Time, s",
        titleFontColor: CTitleColor,
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        interval: 5,
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        //maximum: 30
    },
    axisY: {
        title: "Temperature, C",
        titleFontColor: CTitleColor,
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        //interval: 20,
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 150,
        //maximum: 2
    },
    axisY2: {
        title: "RPM",
        titleFontColor: "#de7d36",
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        //interval: 2000,
        lineColor: "#de7d36",
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        //maximum: 9500
    },
    legend: { horizontalAlign: "center", fontColor: CTitleColor },
    toolTip: { shared: true },
    data: [
        {
            type: "spline",
            showInLegend: true,
            name: "Fuel",
            color: "#7f6084",
            dataPoints: Application.Measurements.TFuel
        },
        {
            type: "spline",
            showInLegend: true,
            name: "Exhaust Gas",
            color: "#6a6a6a",
            dataPoints: Application.Measurements.TExhaustGas
        },
        {
            type: "spline",
            name: "RPM",
            color: "#de7d36",
            axisYType: "secondary",
            dataPoints: Application.Measurements.RPM
        }
    ]
};
Application.Charts.Configs.Pressure = {
    animationEnabled: true,
    title: { text: 'Pressure', fontColor: CTitleColor, fontSize: CTitleSize },
    backgroundColor: CBGColor,
    axisX: {
        title: "Time, s",
        titleFontColor: CTitleColor,        
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        interval: 5,
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        //maximum: 30
    },
    axisY: {
        title: "Pressure, bar",
        titleFontColor: CTitleColor,        
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        //interval: 5,
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        //maximum: 10
    },
    axisY2: {
        title: "RPM",
        titleFontColor: "#de7d36",
        titleFontSize: CATitleSize,
        labelFontSize: CALabelSize,
        //interval: 2000,
        lineColor: "#de7d36",
        tickThickness: 1,
        tickColor: "#5d5d5d",
        gridThickness: 1,
        gridColor: "#5d5d5d",
        minimum: 0,
        //maximum: 9500
    },
    legend: { horizontalAlign: "center", fontColor: CTitleColor },
    toolTip: { shared: true },
    data: [
        {
            type: "spline",
            showInLegend: true,
            name: "Oil",
            color: "#86b402",
            dataPoints: Application.Measurements.POil
        },
        {
            type: "spline",
            showInLegend: true,
            name: "Exhaust Gas",
            color: "#6a6a6a",
            dataPoints: Application.Measurements.PExhaustGas
        },
        {
            type: "spline",
            name: "RPM",
            color: "#de7d36",
            axisYType: "secondary",
            dataPoints: Application.Measurements.RPM
        }
    ]
};