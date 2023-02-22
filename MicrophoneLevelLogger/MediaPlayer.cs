using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace MicrophoneLevelLogger;

public class MediaPlayer : IMediaPlayer
{
    private readonly ISpeaker _speaker;
    public MediaPlayer(ISpeaker speaker)
    {
        _speaker = speaker;
    }

    public Task PlayLoopingAsync(CancellationToken token)
    {
        using var emurator = new MMDeviceEnumerator();
        var mmDevice = emurator.GetDevice(_speaker.Id.AsPrimitive());
        IWavePlayer wavePlayer = new WasapiOut(mmDevice, AudioClientShareMode.Shared, false, 0);

        WaveStream waveStream = new LoopStream(new WaveFileReader(Properties.Resources.吾輩は猫である));

        token.Register(() =>
        {
            wavePlayer.Stop();
            wavePlayer.Dispose();

            mmDevice.Dispose();
            waveStream.Dispose();
        });

        // 出力に入力を接続して再生開始
        wavePlayer.Init(waveStream);
        wavePlayer.Play();

        return Task.CompletedTask;
    }

    public class LoopStream : WaveStream
    {
        readonly WaveStream _sourceStream;

        public LoopStream(WaveStream sourceStream)
        {
            _sourceStream = sourceStream;
        }

        public override WaveFormat WaveFormat => _sourceStream.WaveFormat;

        public override long Length => _sourceStream.Length;

        public override long Position
        {
            get => _sourceStream.Position;
            set => _sourceStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                var bytesRead = _sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (_sourceStream.Position == 0)
                    {
                        // something wrong with the source stream
                        break;
                    }
                    // loop
                    _sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }
}