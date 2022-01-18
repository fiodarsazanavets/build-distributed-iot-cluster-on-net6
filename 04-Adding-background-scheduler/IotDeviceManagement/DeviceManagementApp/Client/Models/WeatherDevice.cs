namespace DeviceManagementApp.Client.Models;

internal record WeatherDevice : Device
{
    public WeatherDevice(string serialNumber, DeviceType deviceType) : 
        base(serialNumber, deviceType)
    {
        WeatherMetrics = new SensorMetrics
        {
            Pressure = 0,
            Humidity = 0,
            Temperature = 0
        };
    }

    public SensorMetrics WeatherMetrics { get; set; }
}
