using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.Wave;
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
        
        _disposable = _bufferedObservable
            .Select(normalize.Filter)
            .Select(fastTimeWeighting.Filter)
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

    private List<double> _buffer = new();

    /// <summary>
    /// RMS（Root Mean Square）を用いて、サンプリング間隔ごとのデシベル値を用いる場合
    /// </summary>
    /// <param name="samples"></param>
    // private void OnNext(WaveInEventArgs e)
    // {
    //     // "scroll" the whole chart to the left
    //     Array.Copy(LiveData, 1, LiveData, 0, LiveData.Length - 1);
    //
    //     int bytesPerSample = 2; // 16 bit samples
    //     int numSamples = e.BytesRecorded / bytesPerSample;
    //
    //     double rms = 0;
    //
    //     for (int i = 0; i < numSamples; i++)
    //     {
    //         short sample = BitConverter.ToInt16(e.Buffer, i * bytesPerSample);
    //         rms += sample * sample;
    //     }
    //
    //     rms = Math.Sqrt(rms / numSamples);
    //     double rmsNormalized = rms / 32768.0; // normalize to 0-1 range
    //
    //     double decibel = 20 * Math.Log10(rmsNormalized);
    //
    //     // Prevent NaN due to log10(0)
    //     if (double.IsNaN(decibel))
    //     {
    //         decibel = Decibel.Minimum.AsPrimitive();
    //     }
    //     
    //     // place the newest data point at the end
    //     LiveData[^1] = decibel;
    // }

    private void OnNext(double[] samples)
    {
        // "scroll" the whole chart to the left
        Array.Copy(LiveData, 1, LiveData, 0, LiveData.Length - 1);

        double rms = 0;

        foreach (var sample in samples)
        {
            rms += sample * sample;
        }
    
        rms = Math.Sqrt(rms / samples.Length);

        double decibel = 20 * Math.Log10(rms);
    
        // Prevent NaN due to log10(0)
        if (double.IsNaN(decibel) 
            || double.IsNegativeInfinity(decibel)
            || decibel < Decibel.MinimumValue)
        {
            decibel = Decibel.MinimumValue;
        }
        
        // place the newest data point at the end
        LiveData[^1] = decibel;
        // _buffer.AddRange(decibels);
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