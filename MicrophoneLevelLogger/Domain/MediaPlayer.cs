using System.Media;

namespace MicrophoneLevelLogger.Domain;

public class MediaPlayer : IMediaPlayer
{
    private SoundPlayer? _player;

    public Task PlayLoopingAsync()
    {
        _player = new(Properties.Resources.吾輩は猫である);
        _player.PlayLooping();
        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        lock (this)
        {
            if (_player is not null)
            {
                _player.Stop();
                _player.Dispose();
                _player = null;
            }
        }
        return Task.CompletedTask;
    }
}