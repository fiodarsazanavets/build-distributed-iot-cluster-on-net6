global using NetCoreAudio;
using AudioPlayerApp;
using CommonDeviceComponents;
using Microsoft.AspNetCore.SignalR.Client;

var cts = new CancellationTokenSource();

Console.CancelKeyPress += new ConsoleCancelEventHandler((s, a) => {
    a.Cancel = true;
    cts.Cancel();
});

var holdOffPlayback = false;
var timeoutSeconds = 60;
var fileManager = new FileManager();
var audioManager = new AudioManager();

var serialNumber = args[0];
var signalrHubUrl = args[1];
var areaName = args[2];
var locationName = args[3];

var clientWrapper = new SignalRClientWrapper(serialNumber, DeviceType.AudioPlayer, signalrHubUrl);
var hubConnection = clientWrapper.GetHubConnection();

hubConnection.On<byte[]>("TriggerPlayback", async (data) => TriggerPlayback(data));
hubConnection.On<bool>("UpdatePlaybackStatus", (playing) => holdOffPlayback = playing);

await clientWrapper.ConnectDevice();

await hubConnection.InvokeAsync("RegisterAudioPlayer", areaName, locationName);

while (!cts.IsCancellationRequested)
{
    await Task.Delay(TimeSpan.FromSeconds(30));
}

async Task TriggerPlayback(byte[] content)
{
    var receiveTime = DateTimeOffset.Now;

    if (holdOffPlayback)
        Console.WriteLine("Other device is playing audio. Waiting...");

    while (holdOffPlayback)
    {    
        if (DateTimeOffset.Now.AddSeconds(-timeoutSeconds) > receiveTime)
            holdOffPlayback = false;

        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    fileManager?.CreateFile(content);

    if (hubConnection?.State == HubConnectionState.Connected)
        await hubConnection.InvokeAsync("BroadcastPlaybackStatus", areaName, true);
    Console.WriteLine("Playback Started");

    if (audioManager != null)
        await audioManager.PlayAudio();

    Console.WriteLine("Playback Finished");

    if (hubConnection?.State == HubConnectionState.Connected)
        await hubConnection.InvokeAsync("BroadcastPlaybackStatus", areaName, false);
}