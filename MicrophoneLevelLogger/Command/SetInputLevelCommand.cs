using MicrophoneLevelLogger.Domain;
using MicrophoneLevelLogger.View;
using Sharprompt;
using System.ComponentModel.DataAnnotations;

namespace MicrophoneLevelLogger.Command;

public class SetInputLevelCommand : ICommand
{
    private readonly ISetInputLevelView _view;
    private readonly IAudioInterfaceProvider _audioInterfaceProvider;

    public SetInputLevelCommand(
        ISetInputLevelView view, 
        IAudioInterfaceProvider audioInterfaceProvider)
    {
        _view = view;
        _audioInterfaceProvider = audioInterfaceProvider;
    }

    public string Name => "Set input level      : 指定マイクを入力レベルを変更する。";
    public Task ExecuteAsync()
    {
        var audioInterface = _audioInterfaceProvider.Resolve();
        var microphone = _view.SelectMicrophone(audioInterface);
        var inputLevel = _view.InputInputLevel();
        microphone.VolumeLevel = new(inputLevel);

        _view.NotifyMicrophonesInformation(audioInterface);

        return Task.CompletedTask;;
    }
}

public interface ISetInputLevelView : IMicrophoneView
{
    IMicrophone SelectMicrophone(IAudioInterface audioInterface);
    float InputInputLevel();
}

public class SetInputLevelView : MicrophoneView, ISetInputLevelView
{
    public IMicrophone SelectMicrophone(IAudioInterface audioInterface)
    {
        return Prompt.Select(
            "マイクを選択してください。",
            audioInterface.Microphones);
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
