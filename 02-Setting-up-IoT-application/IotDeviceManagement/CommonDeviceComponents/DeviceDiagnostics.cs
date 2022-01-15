namespace CommonDeviceComponents;

internal record DeviceDiagnostics
{
    public double CpuUsage { get; init; }
    public double DiskUsage { get; init; }
    public double MemoryUsage { get; init; }
    public float FirmwareVersion { get; init; }
    public bool UpdatingFirmware { get; init; }
}