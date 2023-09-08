using System.Reactive.Linq;
using NAudio.Wave;

namespace Specter;

public class WaveInToInputLevelObservable : IObservable<Decibel>
{
    private readonly IObservable<Decibel> _observable;
    public WaveInToInputLevelObservable(
        IDevice device,
        WaveFormat waveFormat,
        RefreshRate refreshRate)
    {
        var bufferedObservable = new BufferedObservable(
            device.WaveInput, 
            waveFormat, 
            refreshRate);
        var normalize = new Normalize(waveFormat);
        var fastTimeWeighting = new FastTimeWeighting(waveFormat);
        var fft = new FastFourierTransform(waveFormat);
        var converter = new DecibelConverter();
        _observable = bufferedObservable
            .Select(normalize.Filter)
            .Select(fastTimeWeighting.Filter)
            .Select(fft.Transform)
            .Select(converter.Convert)
            .Select(x => new Decibel(x));
    }

    public IDisposable Subscribe(IObserver<Decibel> observer) => _observable.Subscribe(observer);
}