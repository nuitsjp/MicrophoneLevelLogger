using System.Media;

namespace MicrophoneLevelLogger;

public class MediaPlayer : IMediaPlayer
{
    public Task PlayLoopingAsync(CancellationToken token)
    {
        SoundPlayer player = new(Properties.Resources.吾輩は猫である);
        player.PlayLooping();

        token.Register(() =>
        {
            player.Stop();
            player.Dispose();
        });
        return Task.CompletedTask;
    }
}