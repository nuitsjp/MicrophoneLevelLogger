using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Media;

namespace MicrophoneLevelLogger;

public class MediaPlayer : IMediaPlayer
{
    private readonly MMDevice _mmDevice;
    private readonly IWavePlayer _wavePlayer;
    private readonly WaveStream _waveStream = new LoopStream(new AudioFileReader("吾輩は猫である.wav"));

    public MediaPlayer(MMDevice mmDevice)
    {
        _mmDevice = mmDevice;
        _wavePlayer = new WasapiOut(mmDevice, AudioClientShareMode.Shared, false, 0);
    }

    public Task PlayLoopingAsync(CancellationToken token)
    {
        token.Register(Stop);

        // 出力に入力を接続して再生開始
        _wavePlayer.Init(_waveStream);
        _wavePlayer.Play();

        return Task.CompletedTask;
    }

    public void Stop()
    {
        _wavePlayer.Stop();
    }

    public void Dispose()
    {
        _mmDevice.Dispose();
        _wavePlayer.Dispose();
        _waveStream.Dispose();
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
