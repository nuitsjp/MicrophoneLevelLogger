using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command.DisplayMeasurements;

public interface IDisplayMeasurementsView
{
    void NotifyResult(AudioInterfaceInputLevels audioInterfaceInputLevels);
}