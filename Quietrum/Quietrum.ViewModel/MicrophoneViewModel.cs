namespace Quietrum.ViewModel;

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

    public MicrophoneId Id => _microphone.Id;
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