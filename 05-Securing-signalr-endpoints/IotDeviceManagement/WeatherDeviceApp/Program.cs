using CommonDeviceComponents;
using Microsoft.AspNetCore.SignalR.Client;

var cts = new CancellationTokenSource();

Console.CancelKeyPress += new ConsoleCancelEventHandler((s, a) => {
    a.Cancel = true;
    cts.Cancel();
});

var serialNumber = args[0];
var signalrHubUrl = args[1];

var clientWrapper = new SignalRClientWrapper(serialNumber, DeviceType.WeatherSensor, signalrHubUrl);
var hubConnection = clientWrapper.GetHubConnection();
hubConnection.On("RequestMetricUpdate", async () => await SendMetrics());

await clientWrapper.ConnectDevice();

while (!cts.IsCancellationRequested)
{
    await SendMetrics();
    await Task.Delay(TimeSpan.FromMinutes(1));
}

async Task SendMetrics()
{
    var rand = new Random();

    var diagnostics = new SensorMetrics
    {
        Temperature = rand.NextDouble() * 10,
        Humidity = rand.NextDouble() * 100,
        Pressure = rand.NextDouble() * 1000
        
    };

    if (hubConnection != null)
        await hubConnection.SendAsync("UpdateMetrics", diagnostics);
}

Console.ReadKey();