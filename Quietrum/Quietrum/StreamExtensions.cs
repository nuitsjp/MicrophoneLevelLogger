using System.Reactive.Linq;

namespace Quietrum;

public static class StreamExtensions
{
    public static IObservable<(byte[] Bytes, int Size)> ConvertStreamToReactive(this Stream stream, int bufferSize = 4096)
    {
        return Observable.Create<(byte[], int)>(async (observer, cancellationToken) =>
        {
            var buffer = new byte[bufferSize]; // Adjust buffer size as necessary

            try
            {
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    var chunk = new byte[bytesRead];
                    Array.Copy(buffer, chunk, bytesRead);
                    observer.OnNext((chunk, bytesRead));
                }

                observer.OnCompleted();
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
            }
        });
    }
}