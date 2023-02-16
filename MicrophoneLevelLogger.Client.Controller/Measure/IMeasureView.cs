﻿namespace MicrophoneLevelLogger.Client.Controller.Measure;

public interface IMeasureView : IMicrophoneView
{
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    int InputSpan();
    bool ConfirmPlayMedia();
    bool ConfirmReady();
    void Wait(TimeSpan timeout);
    void NotifyResult(AudioInterfaceInputLevels audioInterfaceInputLevels);
}