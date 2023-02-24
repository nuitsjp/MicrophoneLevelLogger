using System.ComponentModel.DataAnnotations;
using MicrophoneLevelLogger.Client.Controller.SetInputLevel;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// マイクの入力レベル設定ビュー。
/// </summary>
public class SetInputLevelView : MicrophoneView, ISetInputLevelView
{
    /// <summary>
    /// マイクを選択する。
    /// </summary>
    /// <param name="audioInterface"></param>
    /// <returns></returns>
    public IMicrophone SelectMicrophone(IAudioInterface audioInterface)
    {
        return Prompt.Select(
            "マイクを選択してください。",
            audioInterface.GetMicrophones());
    }

    /// <summary>
    /// 入力レベルを入力する。
    /// </summary>
    /// <returns></returns>
    public float InputInputLevel()
    {
        return Prompt.Input<float>(
            "入力レベルを指定してください（0～1)",
            null,
            null,
            new List<Func<object, ValidationResult>>
            {
                o =>
                {
                    var inputLevel = (float) o;
                    if (inputLevel.Between(0, 1))
                    {
                        return ValidationResult.Success!;
                    }

                    return new ValidationResult("入力レベルは0～1の間で入力してください。");
                }
            });
    }
}