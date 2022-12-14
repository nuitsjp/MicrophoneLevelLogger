using NAudio.CoreAudioApi;

namespace MicrophoneNoiseAnalyzer.Domain;

public class Microphones : IMicrophones
{
    public Microphones()
    {
        Devices = new MMDeviceEnumerator()
            .EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
            .Select(x => (IMicrophone)new Microphone(x))
            .ToList();
    }

    public void Dispose()
    {
        foreach (var microphone in Devices)
        {
            try
            {
                microphone.Dispose();
            }
            catch
            {
                // ignore
            }
        }
    }

    public IReadOnlyList<IMicrophone> Devices { get; }
    public void Activate()
    {
        foreach (var microphone in Devices)
        {
            microphone.Activate();
        }
    }

    public void StartRecording()
    {
        foreach (var microphone in Devices)
        {
            microphone.StartRecording();
        }
    }

    public IEnumerable<IMasterPeakValues> StopRecording()
    {
        foreach (var microphone in Devices)
        {
            yield return microphone.StopRecording();
        }
    }

    public void Calibrate()
    {
        // キャリブレーション開始時に、すべてのマイクの入力レベルをMaxにする。
        foreach (var microphone in Devices)
        {
            microphone.MasterVolumeLevelScalar = 1;
        }

        var reference = SelectReference();
    }

    public void Deactivate()
    {
        foreach (var microphone in Devices)
        {
            microphone.Deactivate();
        }
    }

    /// <summary>
    /// 基準となるマイクを決定する。
    /// </summary>
    /// <remarks>
    /// マイクは音量をブーストできないものも多いため、基本的には入力レベルを下げて
    /// もっとも入力レベルの小さいマイクに併せる必要があります。
    /// そのため、
    /// </remarks>
    private IMicrophone SelectReference()
    {
        StartRecording();

        Thread.Sleep(TimeSpan.FromSeconds(1));

        var values = StopRecording();

        return values
            .MinBy(x => x.PeakValues.Min())!
            .Microphone;
    }
}