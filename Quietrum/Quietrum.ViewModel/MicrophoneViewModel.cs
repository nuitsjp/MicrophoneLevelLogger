using CommunityToolkit.Mvvm.ComponentModel;

namespace Quietrum.ViewModel;

public partial class MicrophoneViewModel : ObservableObject, IDisposable
{
    private readonly IMicrophone _microphone;
    private readonly RecordingConfig _recordingConfig;
    private IObservable<byte[]>? _observable;
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
        LiveData = new double[(int)(_recordingConfig.RecordingSpan / _recordingConfig.RecordingInterval)];
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
        _observable = _microphone.StartRecording(_recordingConfig.WaveFormat, _recordingConfig.RecordingInterval);
        _disposable = _observable.Subscribe(OnNext);
    }

    public void StopMonitoring()
    {
        _microphone.StopRecording();
        _disposable?.Dispose();
        _observable = null;
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
    private void OnNext(byte[] bytes)
    {
        // "scroll" the whole chart to the left
        Array.Copy(LiveData, 1, LiveData, 0, LiveData.Length - 1);

        var decibels = new double[bytes.Length / _recordingConfig.BytesPerSample];
        for (var index = 0; index < bytes.Length; index += _recordingConfig.BytesPerSample)
        {
            int value = BitConverter.ToInt16(bytes, index);
            var decibel = 20 * Math.Log10((double)value / short.MaxValue);
            if (decibel < Decibel.Minimum.AsPrimitive()) decibel = Decibel.Minimum.AsPrimitive();
            if (double.IsNaN(decibel))
            {
                decibel = Decibel.Minimum.AsPrimitive();
            }
            decibels[index / _recordingConfig.BytesPerSample] = decibel;
        }

        var data = decibels.Max();

        // place the newest data point at the end
        LiveData[^1] = data;
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