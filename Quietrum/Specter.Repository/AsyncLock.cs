﻿namespace Specter.Repository;

public sealed class AsyncLock
{
    private readonly System.Threading.SemaphoreSlim _semaphore 
        = new System.Threading.SemaphoreSlim(1, 1);
    private readonly Task<IDisposable> _releaser;

    public AsyncLock()
    {
        _releaser = Task.FromResult((IDisposable)new Releaser(this));
    }

    public Task<IDisposable> LockAsync()
    {
        var wait = _semaphore.WaitAsync();
        return wait.IsCompleted ?
            _releaser :
            wait.ContinueWith(
                (_, state) => (IDisposable)state!,
                _releaser.Result, 
                System.Threading.CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously, 
                TaskScheduler.Default
            )!;
    }
    private sealed class Releaser : IDisposable
    {
        private readonly AsyncLock _toRelease;
        internal Releaser(AsyncLock toRelease) { _toRelease = toRelease; }
        public void Dispose() { _toRelease._semaphore.Release(); }
    }
}