namespace MicrophoneLevelLogger;

public interface IAudioInterfaceLoggerProvider
{
    IAudioInterfaceLogger ResolveLocal(IAudioInterface audioInterface, string? recordName);
    IAudioInterfaceLogger ResolveLocal(params IMicrophone[] microphones);
    IAudioInterfaceLogger ResolveRemote(string? recordName);
}