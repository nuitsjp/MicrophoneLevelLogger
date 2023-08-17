namespace Specter;

public static class DisposableExtensions
{
    public static void Dispose(this IEnumerable<IDisposable> disposables)
    {
        foreach (var disposable in disposables)
        {
            try
            {
                disposable.Dispose();
            }
            catch
            {
                // ignore
            }
        }
    }
}