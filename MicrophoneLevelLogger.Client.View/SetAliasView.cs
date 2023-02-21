using MicrophoneLevelLogger.Client.Controller.SetAlias;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class SetAliasView : ISetAliasView
{
    public IMicrophone SelectMicrophone(IAudioInterface audioInterface)
    {
        var microphones = audioInterface.GetMicrophones().ToArray();
        var selected = Prompt.Select<string>("マイクを選択してください。", microphones.Select(x => x.Name));
        return microphones.Single(x => x.Name == selected);
    }

    public string InputAlias(IMicrophone microphone)
    {
        return Prompt.Input<string>("別名を入力してください。", microphone.SystemName);
    }
}