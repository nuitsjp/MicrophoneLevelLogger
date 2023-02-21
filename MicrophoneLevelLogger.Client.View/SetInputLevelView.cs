using System.ComponentModel.DataAnnotations;
using MicrophoneLevelLogger.Client.Controller.SetInputLevel;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class SetInputLevelView : MicrophoneView, ISetInputLevelView
{
    public IMicrophone SelectMicrophone(IAudioInterface audioInterface)
    {
        return Prompt.Select(
            "マイクを選択してください。",
            audioInterface.GetMicrophones());
    }

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
                    if (0 <= inputLevel && inputLevel <= 1)
                    {
                        return ValidationResult.Success!;
                    }

                    return new ValidationResult("入力レベルは0～1の間で入力してください。");
                }
            });
    }
}