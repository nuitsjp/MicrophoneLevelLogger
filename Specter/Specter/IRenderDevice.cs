namespace Specter;

public interface IRenderDevice : IDevice
{
    Task PlayLoopingAsync(CancellationToken token);
}