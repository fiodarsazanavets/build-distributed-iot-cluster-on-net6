namespace AudioPlayerApp;

internal interface IFileManager
{
    void CreateFile(byte[] content);
}

internal class FileManager : IFileManager
{
    public void CreateFile(byte[] content)
    {
        File.WriteAllBytes(Constants.AudioFileName, content);
    }
}
