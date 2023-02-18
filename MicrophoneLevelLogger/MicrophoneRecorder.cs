using NAudio.Wave;

namespace MicrophoneLevelLogger;

public class MicrophoneRecorder : IMicrophoneRecorder
{
    private readonly Stream _waveWriter;
    private readonly Fft _fft;

    public MicrophoneRecorder(IMicrophone microphone, DirectoryInfo? directoryInfo)
    {
        Microphone = microphone;

        _waveWriter = directoryInfo is not null
            ? new WaveFileWriter(Path.Join(directoryInfo.FullName, $"{Microphone.Name}.wav"), Microphone.WaveFormat)
            : Stream.Null;
        _fft = new Fft(Microphone.WaveFormat);
    }
    public IMicrophone Microphone { get; }

    public Decibel Max { get; private set; } = Decibel.Min;
    public Decibel Avg { get; private set; } = Decibel.Min;
    public Decibel Min { get; private set; } = Decibel.Min;

    public async Task StartAsync(CancellationToken token)
    {
        await Microphone.ActivateAsync();
        var observerDisposable = Microphone.Subscribe(OnNextWaveInput);

        async void Callback()
        {
            observerDisposable.Dispose();

            // ReSharper disable once MethodSupportsCancellation
            await _waveWriter.FlushAsync();
            await _waveWriter.DisposeAsync();
            Microphone.Deactivate();
        }

        token.Register(Callback);
    }

    public void Dispose()
    {
        Microphone.Dispose();
    }
    private void OnNextWaveInput(WaveInput waveInput)
    {
        _waveWriter.Write(waveInput.Buffer, 0, waveInput.BytesRecorded);
        var decibels =
            AWeighting.Instance.Filter(
                _fft.Transform(waveInput.Buffer, waveInput.BytesRecorded));
        Min = new Decibel(decibels.Min(x => x.Decibel));
        Avg = new Decibel(decibels.Average(x => x.Decibel));
        Max = new Decibel(decibels.Max(x => x.Decibel));
    }
}