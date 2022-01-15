using Microsoft.AspNetCore.SignalR.Client;

namespace CommonDeviceComponents;

public interface ISignalRClientWrapper
{
    HubConnection GetHubConnection();
    Task ConnectDevice();
    Task SendDiagnostics();
}

public class SignalRClientWrapper : ISignalRClientWrapper, IAsyncDisposable
{
    private readonly string serialNumber;
    private readonly DeviceType deviceType;
    private readonly HubConnection hubConnection;

    private bool updatingFirmware;
    private float firmwareVersion;

    public SignalRClientWrapper(
        string serialNumber,
        DeviceType deviceType,
        string signalrServerUrl)
    {
        this.serialNumber = serialNumber;
        this.deviceType = deviceType;
        updatingFirmware = false;
        firmwareVersion = 1.0f;

        hubConnection = new HubConnectionBuilder()
                            .WithUrl(signalrServerUrl)
                            .Build();
        hubConnection.On("RequestDiagnostics", async () => await SendDiagnostics());
        hubConnection.On("RequestFirmwareUpdate", async () => await UpdateFirmware());
        hubConnection.On<float>("UpdateFirmwareVersion", (newVersion) => firmwareVersion = newVersion);
    }

    public HubConnection GetHubConnection()
    {
        return hubConnection;
    }

    public async Task ConnectDevice()
    {
        await hubConnection.SendAsync("ConnectDevice", serialNumber, deviceType);
    }

    public async Task SendDiagnostics()
    {
        var rand = new Random();

        var diagnostics = new DeviceDiagnostics
        {
            CpuUsage = rand.NextDouble() * 100,
            MemoryUsage = rand.NextDouble() * 100,
            DiskUsage = rand.NextDouble() * 100,
            FirmwareVersion = firmwareVersion,
            UpdatingFirmware = updatingFirmware
        };

        await hubConnection.SendAsync("UpdateDiagnostics", diagnostics);
    }

    public async ValueTask DisposeAsync()
    {
        await hubConnection.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    private async Task UpdateFirmware()
    {
        if (updatingFirmware)
        {
            Console.WriteLine("Firmware update is already in progress.");
            return;
        }

        updatingFirmware = true;

        var cancellationTokenSource = new CancellationTokenSource();

        var stream = hubConnection.StreamAsync<int>(
                    "UpdateFirmware", firmwareVersion, cancellationTokenSource.Token);

        await foreach (var percentage in stream)
        {
            Console.WriteLine($"Firmware update progress: {percentage}%");
        }

        updatingFirmware = false;
    }
}

