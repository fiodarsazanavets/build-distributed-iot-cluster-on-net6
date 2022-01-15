using System.Runtime.CompilerServices;

namespace DeviceManagementApp.Server.Hubs;

public class DevicesHub : Hub
{
    public async Task ConnectDevice(string serialNumber, DeviceType deviceType)
    {
        Console.WriteLine($"Device connected. Serial number: {serialNumber}. Device type: {deviceType}.");

        await Clients.Group("Managers")
            .SendAsync("SetDeviceConnected", Context.ConnectionId, serialNumber, deviceType);
    }

    public async Task UpdateMetrics(SensorMetrics metrics)
    {
        Console.WriteLine($"Metrics received from client {Context.ConnectionId}.");

        await Clients.Group("Managers")
            .SendAsync("UpdateMetrics", Context.ConnectionId, metrics);
    }

    public async Task UpdateDiagnostics(DeviceDiagnostics diagnostics)
    {
        Console.WriteLine($"Diagnostics received from client {Context.ConnectionId}.");

        await Clients.Group("Managers")
            .SendAsync("UpdateDiagnostics", Context.ConnectionId, diagnostics);
    }

    public async IAsyncEnumerable<int> UpdateFirmware(
            float firmwareVersion,
            [EnumeratorCancellation]
            CancellationToken cancellationToken)
    {
        for (var i = 0; i <= 100; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return i;
            await Clients.Group("Managers")
                .SendAsync("NotifyFirmwareUpdate", Context.ConnectionId, i, cancellationToken: cancellationToken);
            await Task.Delay(10, cancellationToken);
        }

        await Clients.Caller
            .SendAsync("UpdateFirmwareVersion", firmwareVersion + 0.1, cancellationToken: cancellationToken);
    }

    public async Task RequestMetricUpdate(string connectionId)
    {
        await Clients.Client(connectionId).SendAsync("RequestMetricUpdate");
    }

    public async Task RequestDiagnostics(string connectionId)
    {
        await Clients.Client(connectionId).SendAsync("RequestDiagnostics");
    }

    public async Task RequestFirmwareUpdate(string connectionId)
    {
        await Clients.Client(connectionId).SendAsync("RequestFirmwareUpdate");
    }

    public async Task RebootDevice(string connectionId)
    {
        await Clients.Client(connectionId).SendAsync("Reboot");
    }

    public async Task AddClientToManagersGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Mangers");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Client {Context.ConnectionId} disconnected.");

        await Clients.Group("Managers").SendAsync("SetDeviceDisconnected", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}

