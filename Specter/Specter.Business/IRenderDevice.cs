namespace Specter.Business;

public interface IRenderDevice : IDevice
{
    Task PlayLoopingAsync(CancellationToken token);
}