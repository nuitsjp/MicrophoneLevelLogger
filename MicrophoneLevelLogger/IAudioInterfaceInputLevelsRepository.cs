namespace MicrophoneLevelLogger;

public interface IAudioInterfaceInputLevelsRepository
{
    Task<AudioInterfaceInputLevels> LoadAsync();
    Task SaveAsync(AudioInterfaceInputLevels audioInterfaceInputLevels);
    void Remove();

}