using NAudio.CoreAudioApi;

namespace Specter.Business;

/// <summary>
/// スピーカー
/// </summary>
public class Speaker : ISpeaker
{
    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public Speaker(SpeakerId id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// ID
    /// </summary>
    public SpeakerId Id { get; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 出力レベル
    /// </summary>
    public VolumeLevel VolumeLevel
    {
        get
        {
            using var deviceEnumerator = new MMDeviceEnumerator();
            using var device = deviceEnumerator.GetDevice(Id.AsPrimitive());
            return new VolumeLevel(device.AudioEndpointVolume.MasterVolumeLevelScalar);
        }
        set
        {
            using var deviceEnumerator = new MMDeviceEnumerator();
            using var device = deviceEnumerator.GetDevice(Id.AsPrimitive());
            device.AudioEndpointVolume.MasterVolumeLevelScalar = value.AsPrimitive();
        }

    }
}