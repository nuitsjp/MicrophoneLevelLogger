namespace MicrophoneLevelLogger;

public interface IAudioInterfaceProvider
{
    Task<IAudioInterface> ResolveAsync();
}