namespace DeviceManagementApp.Server;

public interface IAudioFileManager
{
    byte[] GetAudio();
}

internal class AudioFileManager : IAudioFileManager
{
    public byte[] GetAudio()
    {
        var fullFileName = $"Audio{Path.DirectorySeparatorChar}audio.mp3";

        if (!File.Exists(fullFileName))
            return Array.Empty<byte>();

        return File.ReadAllBytes(fullFileName);
    }
}