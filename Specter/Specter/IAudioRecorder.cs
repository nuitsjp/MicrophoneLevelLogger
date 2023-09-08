namespace Specter;

public interface IAudioRecorder
{
    void Start();
    Task<AudioRecord> StopAsync();
}