namespace Specter.Business;

public interface IAudioRecorder
{
    void Start();
    AudioRecord Stop();
}