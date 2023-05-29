using System.Reactive.Linq;
using NAudio.Wave;

namespace Quietrum;

public class BufferedObservable : IObservable<short[]>
{
    private readonly IObservable<WaveInEventArgs> _source;
    private readonly int _bufferSize;
    private readonly IObservable<short[]> _observable;

    public BufferedObservable(IObservable<WaveInEventArgs> source, WaveFormat waveFormat, RefreshRate refreshRate)
    {
        _source = source;
        //_bufferSize = (int)(waveFormat.SampleRate * refreshRate.Interval.TotalSeconds);
        _bufferSize = 1024;
        
        _observable = Observable.Create(OnSubscribe());
    }

    private Func<IObserver<short[]>, IDisposable> OnSubscribe()
    {
        return observer =>
        {
            var buffer = new List<short>();

            return _source.Subscribe(arr =>
                {
                    short[] samples = new short[arr.BytesRecorded / 2];
                    Buffer.BlockCopy(arr.Buffer, 0, samples, 0, arr.BytesRecorded);
                    
                    buffer.AddRange(samples);

                    while (buffer.Count >= _bufferSize)
                    {
                        var emitBuffer = buffer.Take(_bufferSize).ToArray();
                        buffer.RemoveRange(0, _bufferSize);

                        observer.OnNext(emitBuffer);
                    }
                },
                observer.OnError,
                observer.OnCompleted);
        };
    }

    public IDisposable Subscribe(IObserver<short[]> observer) => _observable.Subscribe(observer);
}