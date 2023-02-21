using MicrophoneLevelLogger.Client.Controller.EnableMicrophone;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class EnableMicrophoneView : MicrophoneView, IEnableMicrophoneView
{
    public bool TrySelectMicrophone(IAudioInterface audioInterface, Settings settings, out IMicrophone microphone)
    {
        const string cancel = "取りやめる";

        var items = audioInterface.GetMicrophones(MicrophoneStatus.Enable | MicrophoneStatus.Disable)
            .Where(x => settings.DisabledMicrophones.Contains(x.Id))
            .Select(x => x.Name)
            .ToList();
        items.Add(cancel);

        var selected = Prompt.Select("有効化するマイクを選択してください。", items);
        if (selected == cancel)
        {
            microphone = default!;
            return false;
        }

        microphone = audioInterface.GetMicrophones(MicrophoneStatus.Enable | MicrophoneStatus.Disable).Single(x => x.Name == selected);
        return true;

    }
}