using FluentTextTable;
using MicrophoneLevelLogger.Client.Controller.CalibrateInput;
using Sharprompt;

namespace MicrophoneLevelLogger.Client.View;

public class CalibrateInputView : MicrophoneView, ICalibrateInputView
{
    public IMicrophone SelectReference(IAudioInterface audioInterface)
    {
        return Prompt.Select(
            "リファレンスマイクを選択してください。",
            audioInterface.Microphones);
    }

    public IMicrophone SelectTarget(IAudioInterface audioInterface, IMicrophone reference)
    {
        while (true)
        {
            var target =
                Prompt.Select(
                    "キャリブレーション対象のマイクを選択してください。",
                    audioInterface.Microphones);

            if (target.Id != reference.Id)
            {
                return target;
            }
            ConsoleEx.WriteLine();
            ConsoleEx.WriteLine("リファレンスとは異なるマイクを選択してください。");
            ConsoleEx.WriteLine();
        }
    }

    public void NotifyProgress(IMicrophone reference, Decibel referenceDecibel, IMicrophone target, Decibel targetDecibel)
    {
        Build
            .TextTable<MicrophoneMasterVolumeLevelScalar>(builder =>
            {
                builder.Borders.InsideHorizontal.AsDisable();
                builder.Columns.Add(x => x.Label);
                builder.Columns.Add(x => x.Name);
                builder.Columns.Add(x => x.AverageDecibel).FormatAs("{0:0.00}");
                builder.Columns.Add(x => x.InputLevel).FormatAs("{0:0.00}");
            })
            .WriteLine(new []
            {
                new MicrophoneMasterVolumeLevelScalar("リファレンス", reference.Name, reference.VolumeLevel.AsPrimitive(), referenceDecibel.AsPrimitive()),
                new MicrophoneMasterVolumeLevelScalar("ターゲット", target.Name, target.VolumeLevel.AsPrimitive(), targetDecibel.AsPrimitive())
            });
    }

    public void NotifyCalibrated(AudioInterfaceCalibrationValues calibrationValue, IMicrophone microphone)
    {
        lock (this)
        {
            ConsoleEx.WriteLine();
            ConsoleEx.WriteLine();
            ConsoleEx.WriteLine("マイクのキャリブレーションを完了しました。");
            ConsoleEx.WriteLine($"名称      ：{microphone.Name}");
            ConsoleEx.WriteLine($"入力レベル :{microphone.VolumeLevel.AsPrimitive():0.00}");
            ConsoleEx.WriteLine();
        }
    }

    public void NotifyCalibrated(IAudioInterface audioInterface)
    {
        lock (this)
        {
            ConsoleEx.WriteLine();
            ConsoleEx.WriteLine();
            ConsoleEx.WriteLine("マイクのキャリブレーションを完了しました。");

            for (var i = 0; i < audioInterface.Microphones.Count; i++)
            {
                var microphone = audioInterface.Microphones[i];
                ConsoleEx.WriteLine($"{i + 1} = {microphone.Name} 入力レベル：{microphone.VolumeLevel}");
            }
        }
    }

    public class MicrophoneMasterVolumeLevelScalar
    {
        public MicrophoneMasterVolumeLevelScalar(
            string label, 
            string name,
            float inputLevel, 
            double averageDecibel)
        {
            Label = label;
            Name = name;
            InputLevel = inputLevel;
            AverageDecibel = averageDecibel;
        }

        public string Label { get; }
        public string Name { get; }
        public double AverageDecibel { get; }
        public float InputLevel { get; }
    }
}