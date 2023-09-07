using Specter.Business;

namespace Specter;

public interface IDecibelsReaderProvider
{
    public IDecibelsReader Provide(AudioRecord audioRecord, DeviceRecord deviceRecord);
}