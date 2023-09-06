namespace Specter.Business;

public interface IAudioRecorder
{
    void Start();
    Task<AudioRecord> StopAsync();
}