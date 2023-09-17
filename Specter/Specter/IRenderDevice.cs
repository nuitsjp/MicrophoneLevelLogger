namespace Specter;

public interface IRenderDevice : IDevice
{
    void PlayLooping(CancellationToken token);
}