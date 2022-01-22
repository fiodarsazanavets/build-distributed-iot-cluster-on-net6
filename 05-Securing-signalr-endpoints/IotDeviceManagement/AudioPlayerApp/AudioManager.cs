namespace AudioPlayerApp;

internal interface IAudioPlayer
{
    Task PlayAudio();
}

internal class AudioManager : IAudioPlayer
{
    private readonly Player player;

    public AudioManager()
    {
        player = new();
    }

    public async Task PlayAudio()
    {
        await player.Play(Constants.AudioFileName);

        while (player.Playing)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
