using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Kamishibai;
using ScottPlot;

namespace Quietrum.ViewModel;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class MainWindowViewModel : ObservableObject, INavigatedAware
{
    public RecordingConfig RecordingConfig { get; } = RecordingConfig.Default;

    private readonly IAudioInterface _audioInterface;
    [ObservableProperty]
    private TimeSpan _elapsed = TimeSpan.Zero;
    [ObservableProperty]
    private List<MicrophoneViewModel> _microphones;
    private readonly Stopwatch _stopwatch = new();

    public MainWindowViewModel(IAudioInterface audioInterface)
    {
        _audioInterface = audioInterface;
        _microphones = _audioInterface.GetMicrophones()
            .Select(x => new MicrophoneViewModel(x, RecordingConfig))
            .ToList();
    }
        
    public void OnNavigated(PostForwardEventArgs args)
    {
        var tokenSource = new CancellationTokenSource();
        foreach (var microphone in Microphones)
        {
            microphone.StartRecording(tokenSource.Token);
        }
    }
}