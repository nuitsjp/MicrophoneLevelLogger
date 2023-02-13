namespace MicrophoneLevelLogger;

public class AudioInterfaceCalibrationValues
{
    public IList<MicrophoneCalibrationValue> Microphones { get; set; } = new List<MicrophoneCalibrationValue>();

    public void Update(MicrophoneCalibrationValue microphoneCalibrationValue)
    {
        var old = Microphones.SingleOrDefault(x => x.Name == microphoneCalibrationValue.Name);
        if (old is not null)
        {
            Microphones.Remove(old);
        }

        Microphones.Add(microphoneCalibrationValue);
    }
}