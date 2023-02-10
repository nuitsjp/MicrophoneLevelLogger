﻿using FluentTextTable;
using MicrophoneLevelLogger.Command;
using MicrophoneLevelLogger.Domain;

namespace MicrophoneLevelLogger.View;

public class DisplayCalibratesView : IDisplayCalibratesView
{
    public void NotifyResult(AudioInterfaceCalibrationValues calibrates)
    {
        lock (this)
        {
            Build
                .TextTable<MicrophoneCalibrationValue>(builder =>
                {
                    builder.Borders.InsideHorizontal.AsDisable();
                    builder.Columns.Add(x => x.Name);
                    builder.Columns.Add(x => x.MasterVolumeLevelScalar).FormatAs("{0:#.00}");
                })
                .WriteLine(calibrates.Microphones);
        }

    }
}