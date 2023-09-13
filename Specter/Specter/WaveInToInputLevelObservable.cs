using System.Reactive.Linq;
using NAudio.Wave;

namespace Specter;

public class WaveInToInputLevelObservable : IObservable<Decibel>
{
    private readonly IObservable<Decibel> _observable;
    public WaveInToInputLevelObservable(
        IDevice device,
        WaveFormat waveFormat,
        RefreshRate refreshRate,
        IFastFourierTransformSettings settings)
    {
        var bufferedObservable = new BufferedObservable(
            device.WaveInput, 
            waveFormat, 
            refreshRate);
        var normalize = new Normalize(waveFormat);
        var fft = new FastFourierTransform(settings, waveFormat);
        var converter = new DecibelConverter();
        _observable = bufferedObservable
            .Select(normalize.Filter)
            .Select(fft.Transform)
            .Select(converter.Convert)
            .Select(x => new Decibel(x));
    }

    public IDisposable Subscribe(IObserver<Decibel> observer) => _observable.Subscribe(observer);
}