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
    public void StartCapture()
    {
        foreach (var microphone in Devices)
        {
            microphone.StartCapture();
        }
    }

    public void StopCapture()
    {
        foreach (var microphone in Devices)
        {
            microphone.StopCapture();
        }
    }
}