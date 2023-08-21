using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Specter.Business;

public class RenderDevice : Device, IRenderDevice
{
    public RenderDevice(
        DeviceId id,
        string name, 
        string systemName, 
        bool measure, 
        MMDevice mmDevice) : base(id, name, systemName, measure, mmDevice)
    {
    }

    public override DataFlow DataFlow => DataFlow.Render;
    
    /// <summary>
    /// キャンセルされるまでループ再生する。
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task PlayLoopingAsync(CancellationToken token)
    {
        // スピーカーからNAudioで再生するためのプレイヤーを生成する。
        using var enumerator = new MMDeviceEnumerator();
        var mmDevice = enumerator.GetDevice(Id.AsPrimitive());
        IWavePlayer wavePlayer = new WasapiOut(mmDevice, AudioClientShareMode.Shared, false, 0);

        // ループ音源を作成する。
        WaveStream waveStream = new MediaPlayer.LoopStream(new WaveFileReader(Properties.Resources.吾輩は猫である));

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

}