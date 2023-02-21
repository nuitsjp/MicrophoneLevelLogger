using MicrophoneLevelLogger.Client.Controller.DisableMicrophone;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class DisableMicrophoneView : MicrophoneView, IDisableMicrophoneView
{
    public bool TrySelectMicrophone(IAudioInterface audioInterface, out IMicrophone microphone)
    {
        const string cancel = "取りやめる";

        var microphones = audioInterface.GetMicrophones().ToList();
        var items = microphones
            .Select(x => x.Name)
            .ToList();
        items.Add(cancel);

        var selected = Prompt.Select("無効化するマイクを選択してください。", items);
        if (selected == cancel)
        {
            microphone = default!;
            return false;
        }

        microphone = microphones[items.IndexOf(selected)];
        return true;
    }
}