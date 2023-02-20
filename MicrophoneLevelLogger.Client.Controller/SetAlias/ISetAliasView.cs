namespace MicrophoneLevelLogger.Client.Controller.SetAlias;

public interface ISetAliasView
{
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    string InputAlias(IMicrophone microphone);
}