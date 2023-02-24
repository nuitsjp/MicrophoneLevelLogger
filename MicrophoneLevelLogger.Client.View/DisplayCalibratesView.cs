using FluentTextTable;
using MicrophoneLevelLogger.Client.Controller.DisplayCalibrates;

namespace MicrophoneLevelLogger.Client.View;

/// <summary>
/// マイク調整結果ビュー
/// </summary>
public class DisplayCalibratesView : IDisplayCalibratesView
{
    /// <summary>
    /// 調整結果を表示する。
    /// </summary>
    /// <param name="calibrates"></param>
    public void NotifyResult(AudioInterfaceCalibrationValues calibrates)
    {
        lock (this)
        {
            Build
                .TextTable<MicrophoneCalibrationValue>(builder =>
                {
                    builder.Borders.InsideHorizontal.AsDisable();
                    builder.Columns.Add(x => x.Name);
                    builder.Columns.Add(x => x.VolumeLevel).FormatAs("{0:#.00}");
                })
                .WriteLine(calibrates.Microphones);
        }

    }
}