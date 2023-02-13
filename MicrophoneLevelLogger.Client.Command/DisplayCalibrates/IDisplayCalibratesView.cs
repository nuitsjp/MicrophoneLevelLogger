using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Client.Command.DisplayCalibrates;

public interface IDisplayCalibratesView
{
    void NotifyResult(AudioInterfaceCalibrationValues calibrates);
}