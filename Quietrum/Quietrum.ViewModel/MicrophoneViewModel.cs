using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.Wave;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.TinyLinq;

namespace Quietrum.ViewModel;

public partial class MicrophoneViewModel : ObservableObject, IDisposable
{
    private readonly IMicrophone _microphone;
    private readonly RecordingConfig _recordingConfig;
    private IObservable<WaveInEventArgs>? _observable;
    private IObservable<short[]>? _bufferedObservable;
    private IDisposable? _disposable;
    private WaveRecorder? _waveRecorder;
    [ObservableProperty] private string _minus30dB = string.Empty;
    [ObservableProperty] private string _minus40dB = string.Empty;
    [ObservableProperty] private string _minus50dB = string.Empty;

    public MicrophoneViewModel(
        IMicrophone microphone,
        RecordingConfig recordingConfig)
    {
        _microphone = microphone;
        _microphone.ObserveProperty(x => x.VolumeLevel)
            .Subscribe(x => OnPropertyChanged(nameof(VolumeLevel)));
        _recordingConfig = recordingConfig;
        LiveData = new double[(int)(_recordingConfig.RecordingSpan / _recordingConfig.RefreshRate.Interval)];
        Array.Fill(LiveData, Decibel.Minimum.AsPrimitive());
    }

    public MicrophoneId Id => _microphone.Id;

    public string Name
    {
        get => _microphone.Name;
        set
        {
            _microphone.Name = value;
            OnPropertyChanged();
        }
    }

    public string SystemName => _microphone.SystemName;

    /// <summary>
    /// 入力レベル
    /// </summary>
    public string VolumeLevel
    {
        get => (_microphone.VolumeLevel.AsPrimitive() * 100).ToString("0");
        set
        {
            if(int.TryParse(value, out var intValue))
            {
                if (intValue is >= 0 and <= 100)
                {
                    _microphone.VolumeLevel = new VolumeLevel(intValue / 100f);
                }
            }
            OnPropertyChanged();
        }
    }

    public bool Measure
    {
        get => _microphone.Measure;
        set
        {
            _microphone.Measure = value;
            if (_microphone.Measure)
            {
                StartMonitoring();
            }
            else
            {
                StopMonitoring();
            }
            OnPropertyChanged();
        }
    }
    public double[] LiveData { get; }

    public void StartMonitoring()
    {
        _observable = _microphone.StartRecording(_recordingConfig.WaveFormat, _recordingConfig.RefreshRate.Interval);
        _bufferedObservable = new BufferedObservable(
            _observable, 
            _recordingConfig.WaveFormat, 
            _recordingConfig.RefreshRate);
        var normalize = new Normalize(_recordingConfig.WaveFormat);
        var fastTimeWeighting = new FastTimeWeighting(_recordingConfig.WaveFormat);
        var fft = new FastFourierTransform(_recordingConfig.WaveFormat);
        var converter = new DecibelConverter();
        _disposable = _bufferedObservable
            .Select(normalize.Filter)
            .Select(fastTimeWeighting.Filter)
            .Select(fft.Transform)
            .Select(converter.Convert)
            .Subscribe(OnNext);
    }

    public void StopMonitoring()
    {
        _microphone.StopRecording();
        _disposable?.Dispose();
        _observable = null;
        _bufferedObservable = null;
        Array.Fill(LiveData, Decibel.Minimum.AsPrimitive());
    }

    public void StartRecording(DirectoryInfo directoryInfo)
    {
        if (_observable is null)
        {
            throw new InvalidOperationException();
        }

        var fileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, Name + ".wav"));
        _waveRecorder = new WaveRecorder(
            fileInfo,
            _recordingConfig.WaveFormat,
            _observable);
        _waveRecorder.StartRecording();
    }

    public void StopRecording()
    {
        _waveRecorder?.StopRecording();
        _waveRecorder = null;
    }

    private void OnNext(double decibel)
    {
        // "scroll" the whole chart to the left
        Array.Copy(LiveData, 1, LiveData, 0, LiveData.Length - 1);
        LiveData[^1] = decibel;
        // Minus30dB = $"{_buffer.Count(x => -30d < x) / (double)_buffer.Count * 100d:#0.00}%";
        // Minus40dB = $"{_buffer.Count(x => -40d < x) / (double)_buffer.Count * 100d:#0.00}%";
        // Minus50dB = $"{_buffer.Count(x => -50d < x) / (double)_buffer.Count * 100d:#0.00}%";
    }

    public void Dispose()
    {
        _microphone.StopRecording();
        _microphone.Dispose();
    }
}