namespace DeviceManagementApp.Server.Models;

internal class AudioEvent
{
    public AudioEvent(string location, string area, DateTimeOffset eventTime)
    {
        Location = location;
        Area = area;
        EventTime = eventTime;
    }

    public string Location { get; }
    public string Area { get; }
    public DateTimeOffset EventTime { get; }
}