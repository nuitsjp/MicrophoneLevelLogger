using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Kamishibai;
using NAudio.Wave;
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

public class MicrophoneViewModel
{
    private readonly IMicrophone _microphone;
    private readonly RecordingConfig _recordingConfig;

    public MicrophoneViewModel(
        IMicrophone microphone,
        RecordingConfig recordingConfig)
    {
        _microphone = microphone;
        _recordingConfig = recordingConfig;
        LiveData = new double[(int)(_recordingConfig.RecordingSpan / _recordingConfig.RecordingInterval)];
        Array.Fill(LiveData, Decibel.Minimum.AsPrimitive());
    }

    public string Name => _microphone.Name;
    public double[] LiveData { get; }

    public void StartRecording(CancellationToken token)
    {
        var observable = _microphone.StartRecording(_recordingConfig.WaveFormat, _recordingConfig.RecordingInterval, token);
        observable.Subscribe(OnNext);
    }
    
    private void OnNext(byte[] bytes)
    {
        // "scroll" the whole chart to the left
        Array.Copy(LiveData, 1, LiveData, 0, LiveData.Length - 1);

        int peakValue = 0;
        for (int index = 0; index < bytes.Length; index += _recordingConfig.BytesPerSample)
        {
            int value = BitConverter.ToInt16(bytes, index);
            peakValue = Math.Max(peakValue, value);
        }

        double level = (double)peakValue / (double)short.MaxValue;
        double decibel = 20 * Math.Log10(level);

        // place the newest data point at the end
        LiveData[^1] = Math.Max(Math.Min(decibel, 0d), -84d);
    }
}

public record RecordingConfig(
    WaveFormat WaveFormat,
    TimeSpan RecordingSpan,
    TimeSpan RecordingInterval)
{
    public static readonly RecordingConfig Default =
        new(new (48_000, 16, 1), TimeSpan.FromMinutes(2), TimeSpan.FromMilliseconds(50));

    public int RecordingLength => (int)(RecordingSpan / RecordingInterval);
    public int BytesPerSample => WaveFormat.BitsPerSample / 8;
}