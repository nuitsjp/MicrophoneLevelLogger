using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public interface IDisplayMeasurementsView
{
    void NotifyResult(AudioInterfaceInputLevels audioInterfaceInputLevels);
}