using System.Threading;
using System.Threading.Tasks;

namespace Specter.Business;

public interface IRenderDevice : IDevice
{
    Task PlayLoopingAsync(CancellationToken token);
}