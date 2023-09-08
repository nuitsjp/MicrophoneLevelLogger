namespace Specter;

public interface IAudioRecordRepository
{
    Task SaveAsync(AudioRecord audioRecord);
    Task<IEnumerable<AudioRecord>> LoadAsync();
}