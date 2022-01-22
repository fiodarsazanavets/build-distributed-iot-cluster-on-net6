namespace DeviceManagementApp.Server;

public class LocationMapper
{
    private readonly Dictionary<string, string> locations;

    public LocationMapper()
    {
        locations = new();
    }

    public void MapDeviceToLocation(string connectionId, string location)
    {
        locations[location] = connectionId;
    }

    public string? GetConectionId(string location)
    {
        if (locations.ContainsKey(location))
            return locations[location];

        return null;
    }

    public void RemoveLocation(string connectionId)
    {
        var location = locations
                    .Where(l => l.Value == connectionId)
                    .Select(l => l.Key)
                    .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(location))
            locations.Remove(location);
    }
}

