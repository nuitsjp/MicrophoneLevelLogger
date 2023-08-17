using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Quietrum;

/// <summary>
/// メディアプレイヤー
/// </summary>
public class MediaPlayer : IMediaPlayer
{
    /// <summary>
    /// 音源を出力するスピーカー
    /// </summary>
    private readonly ISpeaker _speaker;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="speaker"></param>
    public MediaPlayer(ISpeaker speaker)
    {
        _speaker = speaker;
    }

    /// <summary>
    /// キャンセルされるまでループ再生する。
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task PlayLoopingAsync(CancellationToken token)
    {
        // スピーカーからNAudioで再生するためのプレイヤーを生成する。
        using var emurator = new MMDeviceEnumerator();
        var mmDevice = emurator.GetDevice(_speaker.Id.AsPrimitive());
        IWavePlayer wavePlayer = new WasapiOut(mmDevice, AudioClientShareMode.Shared, false, 0);

        // ループ音源を作成する。
        WaveStream waveStream = new LoopStream(new WaveFileReader(Specter.Business.Properties.Resources.吾輩は猫である));

        // 終了処理を登録する。
        token.Register(() =>
        {
            // リソースを開放する。
            wavePlayer.Stop();
            wavePlayer.Dispose();
            mmDevice.Dispose();
            waveStream.Dispose();
        });

        // 出力に入力を接続して再生を開始する。
        wavePlayer.Init(waveStream);
        wavePlayer.Play();

        return Task.CompletedTask;
    }

    /// <summary>
    /// 元音源ストリームをループで再生するためのストリーム。
    /// </summary>
    public class LoopStream : WaveStream
    {
        /// <summary>
        /// 元音源ストリーム
        /// </summary>
        readonly WaveStream _sourceStream;

        /// <summary>
        /// インスタンスを生成する。
        /// </summary>
        /// <param name="sourceStream"></param>
        public LoopStream(WaveStream sourceStream)
        {
            _sourceStream = sourceStream;
        }

        /// <summary>
        /// WaveFormat
        /// </summary>
        public override WaveFormat WaveFormat => _sourceStream.WaveFormat;

        /// <summary>
        /// 音源の長さ
        /// </summary>
        public override long Length => _sourceStream.Length;

        /// <summary>
        /// 再生中のポジションを取得・設定する。
        /// </summary>
        public override long Position
        {
            get => _sourceStream.Position;
            set => _sourceStream.Position = value;
        }

        /// <summary>
        /// 音源を読み取る。
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
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
                        // 元音源に問題がある場合、再生を停止する。
                        break;
                    }
                    // 末尾まで戻したので先頭に戻す。
                    _sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }

        /// <summary>
        /// リソースを開放する。
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _sourceStream.Dispose();
        }
    }
}