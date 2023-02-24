using MicrophoneLevelLogger.Client.Controller.SetAlias;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// マイクの別名設定ビュー。
/// </summary>
public class SetAliasView : ISetAliasView
{
    /// <summary>
    /// マイクを選択する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <returns></returns>
    public IMicrophone SelectMicrophone(IAudioInterface audioInterface)
    {
        var microphones = audioInterface.GetMicrophones().ToArray();
        var selected = Prompt.Select("マイクを選択してください。", microphones.Select(x => x.Name));
        return microphones.Single(x => x.Name == selected);
    }

    /// <summary>
    /// 別名を入力する。
    /// </summary>
    /// <param name="microphone"></param>
    /// <returns></returns>
    public string InputAlias(IMicrophone microphone)
    {
        return Prompt.Input<string>("別名を入力してください。", microphone.SystemName);
    }
}