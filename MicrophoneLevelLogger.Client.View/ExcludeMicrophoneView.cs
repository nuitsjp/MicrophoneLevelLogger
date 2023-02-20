using Sharprompt;

namespace MicrophoneLevelLogger.Client.Controller.ExcludeMicrophone;

public class ExcludeMicrophoneView : IExcludeMicrophoneView
{
    public bool TrySelectMicrophone(IAudioInterface audioInterface, out IMicrophone microphone)
    {
        const string cancel = "取りやめる";

        var items = audioInterface.Microphones
            .Select(x => x.Name)
            .ToList();
        items.Add(cancel);

        var selected = Prompt.Select("削除する別名を選択してください。", items);
        if (selected == cancel)
        {
            microphone = default!;
            return false;
        }

        microphone = audioInterface.Microphones[items.IndexOf(selected)];
        return true;
    }
}