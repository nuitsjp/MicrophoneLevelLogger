namespace MicrophoneLevelLogger;

public class InputLevelMeter
{
    private readonly IMicrophone _microphone;

    private bool _isRunning;

    private readonly List<double> _maximumDecibels = new();

    public InputLevelMeter(IMicrophone microphone)
    {
        _microphone = microphone;
    }

    public void StartMonitoring()
    {
        _isRunning = true;
        var delay = TimeSpan.FromMilliseconds(IMicrophone.SamplingMilliseconds);
        Task.Run(async () =>
        {
            while (_isRunning)
            {
                lock (_maximumDecibels)
                {
                    _maximumDecibels.Add(_microphone.LatestWaveInput.MaximumDecibel);
                }
            }

            await Task.Delay(delay);
        });
    }

    public MicrophoneInputLevel StopMonitoring()
    {
        _isRunning = false;
        lock (_maximumDecibels)
        {
            return new MicrophoneInputLevel(
                _microphone.Id,
                _microphone.Name,
                _maximumDecibels.Min(),
                _maximumDecibels.Average(),
                _maximumDecibels.Median(),
                _maximumDecibels.Max());
        }
    }
}