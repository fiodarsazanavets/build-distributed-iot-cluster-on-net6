namespace DeviceManagementApp.Server;

internal interface IEventSchedule
{
    AudioEvent? GetNextEvent();
}

internal class EventSchedule : IEventSchedule
{
    private readonly List<AudioEvent> events;

    public EventSchedule()
    {
        events = new();
        events.Add(new AudioEvent("1", "Bracknell", DateTimeOffset.UtcNow.AddSeconds(12)));
        events.Add(new AudioEvent("1", "Ascot", DateTimeOffset.UtcNow.AddSeconds(16)));
        events.Add(new AudioEvent("2", "Ascot", DateTimeOffset.UtcNow.AddSeconds(18)));
        events.Add(new AudioEvent("2", "Bracknell", DateTimeOffset.UtcNow.AddSeconds(45)));
        events.Add(new AudioEvent("1", "Bracknell", DateTimeOffset.UtcNow.AddSeconds(60)));
        events.Add(new AudioEvent("2", "Bracknell", DateTimeOffset.UtcNow.AddSeconds(90)));
    }

    public AudioEvent? GetNextEvent()
    {
        var audioEvent = events.FirstOrDefault(e => e.EventTime < DateTimeOffset.UtcNow);

        if (audioEvent is null)
            return null;

        events.Remove(audioEvent);

        return audioEvent;
    }
}