namespace MicrophoneLevelLogger.Client.Controller.DisplayMeasurements;

public interface IDisplayMeasurementsView
{
    void NotifyResult(AudioInterfaceInputLevels audioInterfaceInputLevels);
}