namespace DeviceManagementApp.Client.Models;

internal record Device
{
    public Device(string serialNumber, DeviceType deviceType)
    {
        SerialNumber = serialNumber;
        DeviceType = deviceType;
        FirmwareUpdating = false;
        FirmwareUpdateProgress = 0;
        Diagnostics = new DeviceDiagnostics
        {
            CpuUsage = 0,
            MemoryUsage = 0,
            DiskUsage = 0
        };
    }

    public string SerialNumber { get; set; }
    public bool FirmwareUpdating { get; set; }
    public int FirmwareUpdateProgress { get; set; }
    public DeviceType DeviceType { get; set; }
    public DeviceDiagnostics Diagnostics { get; set;}
}
