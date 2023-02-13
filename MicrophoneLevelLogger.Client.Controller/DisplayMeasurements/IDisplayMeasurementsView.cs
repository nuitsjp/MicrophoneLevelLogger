namespace MicrophoneLevelLogger.Client.Command.DisplayMeasurements;

public interface IDisplayMeasurementsView
{
    void NotifyResult(AudioInterfaceInputLevels audioInterfaceInputLevels);
}