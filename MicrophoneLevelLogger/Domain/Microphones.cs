using NAudio.CoreAudioApi;

namespace MicrophoneLevelLogger.Domain;

public class Microphones : IMicrophones
{
    public Microphones()
    {
        Devices = new MMDeviceEnumerator()
            .EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
            .Select((x, index) => (IMicrophone)new Microphone(x, index))
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
        var tasks = Devices
            .Select(x => x.ActivateAsync());
        Task.WaitAll(tasks.ToArray());
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