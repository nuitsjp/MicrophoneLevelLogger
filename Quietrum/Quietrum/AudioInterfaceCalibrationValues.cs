namespace Quietrum;

/// <summary>
/// キャリブレーション結果
/// </summary>
public class AudioInterfaceCalibrationValues
{
    /// <summary>
    /// マイク別のキャリブレーション結果
    /// </summary>
    private readonly List<MicrophoneCalibrationValue> _microphones;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="microphones"></param>
    public AudioInterfaceCalibrationValues(IReadOnlyList<MicrophoneCalibrationValue> microphones)
    {
        _microphones = microphones.ToList();
    }

    /// <summary>
    /// マイク別のキャリブレーション結果
    /// </summary>
    public IReadOnlyList<MicrophoneCalibrationValue> Microphones => _microphones;

    /// <summary>
    /// マイクのキャリブレーション結果を更新する。
    /// </summary>
    /// <param name="microphoneCalibrationValue"></param>
    public void Update(MicrophoneCalibrationValue microphoneCalibrationValue)
    {
        var old = Microphones.SingleOrDefault(x => x.Name == microphoneCalibrationValue.Name);
        if (old is not null)
        {
            _microphones.Remove(old);
        }

        _microphones.Add(microphoneCalibrationValue);
    }
}