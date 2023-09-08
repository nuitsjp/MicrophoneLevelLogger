namespace Specter.Repository;

public class DecibelsReaderProvider : IDecibelsReaderProvider
{
    public IDecibelsReader Provide(AudioRecord audioRecord, DeviceRecord deviceRecord)
    {
        var file = Path.Combine(
            AudioRecordInterface.GetAudioRecordPath(audioRecord),
            $"{deviceRecord.Name}.ilv");
        return new DecibelsReader(File.OpenRead(file));
    }
}