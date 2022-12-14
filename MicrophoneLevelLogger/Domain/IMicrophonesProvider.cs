namespace MicrophoneLevelLogger.Domain;

public interface IMicrophonesProvider
{
    IMicrophones Resolve();
}