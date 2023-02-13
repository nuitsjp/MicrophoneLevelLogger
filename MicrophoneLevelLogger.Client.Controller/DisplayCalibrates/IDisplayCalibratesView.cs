namespace MicrophoneLevelLogger.Client.Controller.DisplayCalibrates;

public interface IDisplayCalibratesView
{
    void NotifyResult(AudioInterfaceCalibrationValues calibrates);
}