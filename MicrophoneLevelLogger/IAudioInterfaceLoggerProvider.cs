namespace MicrophoneLevelLogger;

public interface IAudioInterfaceLoggerProvider
{
    IAudioInterfaceLogger ResolveLocal(IAudioInterface audioInterface, string? recordName);
    IAudioInterfaceLogger ResolveRemote(string? recordName);
}