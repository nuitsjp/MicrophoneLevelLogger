using MicrophoneNoiseAnalyzer.Domain;
using MicrophoneNoiseAnalyzer.View;
using NAudio.CoreAudioApi;

namespace MicrophoneNoiseAnalyzer.Command;

public class MicrophoneNoiseAnalyzerCommands : ConsoleAppBase
{
    private readonly IMicrophonesProvider _microphonesProvider;
    private readonly ICalibrationView _view;

    public MicrophoneNoiseAnalyzerCommands(IMicrophonesProvider microphonesProvider, ICalibrationView view)
    {
        _microphonesProvider = microphonesProvider;
        _view = view;
    }

    public void Calibration()
    {
        using IMicrophones microphones = _microphonesProvider.Resolve();

        _view.NotifyMicrophonesInformation(microphones);
        if(_view.ConfirmInvoke() is false)
        {
            return;
        }

        foreach (var microphone in microphones.Devices)
        {
            microphone.MasterVolumeLevelScalar = 1;
        }

        microphones.StartCapture();

        using var timer = 
            new Timer(
                x => _view.NotifyMasterPeakValue((IMicrophones)x!),
                microphones, 
                TimeSpan.Zero, 
                TimeSpan.FromMilliseconds(50));

        Console.ReadLine();

        microphones.StopCapture();
    }
}