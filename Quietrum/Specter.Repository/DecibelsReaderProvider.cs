using Specter.Business;

namespace Specter.Repository;

public class DecibelsReaderProvider : IDecibelsReaderProvider
{
    public IDecibelsReader Provide(AudioRecord audioRecord, DeviceRecord deviceRecord)
    {
        var file = Path.Combine(
            AudioRecordRepository.GetAudioRecordPath(audioRecord),
            $"{deviceRecord.Name}.ilv");
        return new DecibelsReader(File.OpenRead(file));
    }
}