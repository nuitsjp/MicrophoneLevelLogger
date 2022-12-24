using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace MicrophoneLevelLogger.Domain;

public class Microphones : IMicrophones
{
    public Microphones()
    {
        using var enumerator = new MMDeviceEnumerator();
        var mmDevices = enumerator
                .EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
                .ToArray();
        List<IMicrophone> devices = new();
        try
        {
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var capability = WaveIn.GetCapabilities(i);
                var name = capability.ProductName;
                var mmDevice = mmDevices.SingleOrDefault(x => x.FriendlyName == name);
                if (mmDevice is not null)
                {
                    devices.Add(new Microphone(mmDevice.ID, name, i));
                }

            }

            Devices = devices;
        }
        finally
        {
            foreach (var mmDevice in mmDevices)
            {
                mmDevice.DisposeQuiet();
            }
        }

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
}