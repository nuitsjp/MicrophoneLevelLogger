using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command.DisplayCalibrates;

public interface IDisplayCalibratesView
{
    void NotifyResult(AudioInterfaceCalibrationValues calibrates);
}