using FluentTextTable;
using MicrophoneLevelLogger.Command;
using MicrophoneLevelLogger.Domain;
using NAudio.Utils;
using Sharprompt;

namespace MicrophoneLevelLogger.View;

public class MeasureInputLevelView : MicrophoneView, IMeasureInputLevelView
{
    public IMicrophone SelectMicrophone(IAudioInterface audioInterface)
    {
        var microphone = Prompt.Select(
            "計測対象のマイクを選択してください。",
            audioInterface.Microphones);

        if (Prompt.Confirm("マイクに別名を付けますか？", false))
        {
            var name = Prompt.Input<string>("マイクの別名を入力してください。", microphone.Name);
            microphone = new Microphone(microphone.Id, name, microphone.DeviceNumber);
        }

        return microphone;
    }

    public void NotifyResult(AudioInterfaceInputLevels audioInterfaceInputLevels)
    {
        lock (this)
        {
            Build
                .TextTable<MicrophoneInputLevel>(builder =>
                {
                    builder.Borders.InsideHorizontal.AsDisable();
                    builder.Columns.Add(x => x.Name);
                    builder.Columns.Add(x => x.Min).FormatAs("{0:#.00}");
                    builder.Columns.Add(x => x.Avg).FormatAs("{0:#.00}");
                    builder.Columns.Add(x => x.Median).FormatAs("{0:#.00}");
                    builder.Columns.Add(x => x.Max).FormatAs("{0:#.00}");
                })
                .WriteLine(audioInterfaceInputLevels.Microphones);
        }
    }
}