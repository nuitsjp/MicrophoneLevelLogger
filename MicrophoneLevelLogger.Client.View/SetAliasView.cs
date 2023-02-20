using MicrophoneLevelLogger.Client.Controller.SetAlias;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class SetAliasView : ISetAliasView
{
    public IMicrophone SelectMicrophone(IAudioInterface audioInterface)
    {
        var selected = Prompt.Select<string>("マイクを選択してください。", audioInterface.Microphones.Select(x => x.Name));
        return audioInterface
            .Microphones
            .Single(x => x.Name == selected);
    }

    public string InputAlias(IMicrophone microphone)
    {
        return Prompt.Input<string>("別名を入力してください。", microphone.SystemName);
    }
}