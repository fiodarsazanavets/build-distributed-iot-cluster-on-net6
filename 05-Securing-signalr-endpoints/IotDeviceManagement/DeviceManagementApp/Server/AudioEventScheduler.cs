namespace DeviceManagementApp.Server;

internal class AudioEventScheduler : BackgroundService
{
    private readonly IEventSchedule schedule;
    private readonly LocationMapper locationMapper;
    private readonly IAudioFileManager audioManager;
    private readonly IHubContext<DevicesHub> hubContext;

    public AudioEventScheduler(
        IEventSchedule schedule,
        LocationMapper locationMapper,
        IAudioFileManager audioManager,
        IHubContext<DevicesHub> hubContext)
    {
        this.schedule = schedule;
        this.locationMapper = locationMapper;
        this.audioManager = audioManager;
        this.hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var audioEvent = schedule.GetNextEvent();
            var connectionId = audioEvent is not null ? locationMapper.GetConectionId(audioEvent.Location) : null;
            var groupName = audioEvent is not null ? audioEvent.Area : null;
            var audioContent = audioManager.GetAudio();

            if (!string.IsNullOrWhiteSpace(connectionId))
                await hubContext.Clients.Client(connectionId).SendAsync("TriggerPlayback", audioContent, cancellationToken: stoppingToken);
            else if (!string.IsNullOrWhiteSpace(groupName))
                await hubContext.Clients.Group(groupName).SendAsync("TriggerPlayback", audioContent, cancellationToken: stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}