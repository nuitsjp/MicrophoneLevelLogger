using NAudio.Wave;

namespace Quietrum;

/// <summary>
/// マイク
/// </summary>
public interface IMicrophone : IDisposable
{
    /// <summary>
    /// ID
    /// </summary>
    MicrophoneId Id { get; }
    /// <summary>
    /// 名称
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Windows上の名称
    /// </summary>
    string SystemName { get; }
    /// <summary>
    /// ステータス
    /// </summary>
    MicrophoneStatus Status { get; }
    /// <summary>
    /// 入力レベル
    /// </summary>
    VolumeLevel VolumeLevel { get; set; }

    IObservable<byte[]> StartRecording(WaveFormat waveFormat, TimeSpan bufferSpan, CancellationToken cancellationToken);
}