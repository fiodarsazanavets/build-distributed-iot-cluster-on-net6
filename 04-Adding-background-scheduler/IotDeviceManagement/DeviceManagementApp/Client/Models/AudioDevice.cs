namespace DeviceManagementApp.Client.Models;

internal record AudioDevice : Device
{
    public AudioDevice(string serialNumber, DeviceType deviceType) : 
        base(serialNumber, deviceType)
    {
        Area = string.Empty;
        Location = string.Empty;
        PlayingAudio = false;
    }

    public string Area { get; set; }
    public string Location { get; set; }
    public bool PlayingAudio { get; set; }
}
