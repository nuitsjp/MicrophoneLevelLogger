using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.Command;

public interface IDisplayCalibratesView
{
    void NotifyResult(AudioInterfaceCalibrationValues calibrates);
}