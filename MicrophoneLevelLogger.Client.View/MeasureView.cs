using FluentTextTable;
using MicrophoneLevelLogger.Client.Command.Measure;
using MicrophoneLevelLogger.Domain;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class MeasureView : MicrophoneView, IMeasureView
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

    public int InputSpan()
    {
        return Prompt.Input<int>("計測時間[秒]を入力してください。", 30);
    }

    public bool ConfirmPlayMedia()
    {
        return Prompt.Confirm("メディアを再生しますか？", true);
    }

    public bool ConfirmReady()
    {
        return Prompt.Confirm("準備はできましたか？", true);
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