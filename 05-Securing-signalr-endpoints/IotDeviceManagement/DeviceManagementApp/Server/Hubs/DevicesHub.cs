using System.Runtime.CompilerServices;

namespace DeviceManagementApp.Server.Hubs;

[Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults.AuthenticationScheme + "," +
        JwtBearerDefaults.AuthenticationScheme)]
public class DevicesHub : Hub
{
    private const string ManagersGroup = "Managers";
    private const string DevicePolicy = "Device";

    private readonly LocationMapper locationMapper;
    private readonly IAudioFileManager audioManager;

    public DevicesHub(
        LocationMapper locationMapper,
        IAudioFileManager audioManager)
    {
        this.locationMapper = locationMapper;
        this.audioManager = audioManager;
    }

    [Authorize(Policy = DevicePolicy)]
    public async Task ConnectDevice(string serialNumber, DeviceType deviceType)
    {
        Console.WriteLine($"Device connected. Serial number: {serialNumber}. Device type: {deviceType}.");

        await Clients.Group(ManagersGroup)
            .SendAsync("SetDeviceConnected", Context.ConnectionId, serialNumber, deviceType);
    }

    [Authorize(Policy = DevicePolicy)]
    public async Task UpdateMetrics(SensorMetrics metrics)
    {
        Console.WriteLine($"Metrics received from client {Context.ConnectionId}.");

        await Clients.Group(ManagersGroup)
            .SendAsync("UpdateMetrics", Context.ConnectionId, metrics);
    }

    [Authorize(Policy = DevicePolicy)]
    public async Task UpdateDiagnostics(DeviceDiagnostics diagnostics)
    {
        Console.WriteLine($"Diagnostics received from client {Context.ConnectionId}.");

        await Clients.Group(ManagersGroup)
            .SendAsync("UpdateDiagnostics", Context.ConnectionId, diagnostics);
    }

    [Authorize(Policy = DevicePolicy)]
    public async IAsyncEnumerable<int> UpdateFirmware(
            float firmwareVersion,
            [EnumeratorCancellation]
            CancellationToken cancellationToken)
    {
        for (var i = 0; i <= 100; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return i;
            await Clients.Group(ManagersGroup)
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
        await Groups.AddToGroupAsync(Context.ConnectionId, ManagersGroup);
    }

    [Authorize(Policy = DevicePolicy)]
    public async Task RegisterAudioPlayer(string areaName, string locationName)
    {
        locationMapper.MapDeviceToLocation(Context.ConnectionId, locationName);
        await Groups.AddToGroupAsync(Context.ConnectionId, areaName);
        await Clients.Group(ManagersGroup)
            .SendAsync("RegisterAudioPlayer", Context.ConnectionId, areaName, locationName);
    }

    [Authorize(Policy = DevicePolicy)]
    public async Task BroadcastPlaybackStatus(string areaName, bool playing)
    {
        await Clients.Groups(areaName).SendAsync("UpdatePlaybackStatus", playing);
        await Clients.Group(ManagersGroup)
            .SendAsync("UpdatePlaybackStatus", Context.ConnectionId, playing);
    }

    public async Task TriggerAudioPlayback(string connectionId)
    {
        var audioContent = audioManager.GetAudio();
        await Clients.Client(connectionId).SendAsync("TriggerPlayback", audioContent);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Client {Context.ConnectionId} disconnected.");

        await Clients.Group(ManagersGroup).SendAsync("SetDeviceDisconnected", Context.ConnectionId);
        locationMapper.RemoveLocation(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}

