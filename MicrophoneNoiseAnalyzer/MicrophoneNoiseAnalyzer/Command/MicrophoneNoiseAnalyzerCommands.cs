using MicrophoneNoiseAnalyzer.Domain;
using MicrophoneNoiseAnalyzer.View;
using NAudio.CoreAudioApi;
using System.Reflection.Emit;

namespace MicrophoneNoiseAnalyzer.Command;

public class MicrophoneNoiseAnalyzerCommands : ConsoleAppBase
{
    private readonly IMicrophonesProvider _microphonesProvider;
    private readonly ICalibrationView _view;

    public MicrophoneNoiseAnalyzerCommands(IMicrophonesProvider microphonesProvider, ICalibrationView view)
    {
        _microphonesProvider = microphonesProvider;
        _view = view;
    }

    public void Calibrate()
    {
        // すべてのマイクを取得する。
        using IMicrophones microphones = _microphonesProvider.Resolve();

        // 起動時情報を通知する。
        _view.NotifyMicrophonesInformation(microphones);
        if(_view.ConfirmInvoke() is false)
        {
            // 確認でNoが推されたら中断する。
            return;
        }

        // キャリブレーション開始時に、すべてのマイクの入力レベルをMaxにする。
        foreach (var microphone in microphones.Devices)
        {
            microphone.MasterVolumeLevelScalar = 1;
        }

        // キャプチャーを開始する。
        microphones.StartRecording();

        // 画面に入力レベルを通知する。
        _view.StartNotifyMasterPeakValue(microphones);

        Console.ReadLine();

        // 画面の入力レベル通知を停止する。
        _view.StopNotifyMasterPeakValue();

        // キャプチャーを停止する
        var peakValuesList = microphones.StopRecording().ToList();

        // 平均値が最小のマイクの音量を基準値とする
        var referenceLevel = peakValuesList
            .Select(x => x.PeakValues.Average())
            .Min();

        foreach (var peakValues in peakValuesList)
        {
            var microphone = peakValues.Microphone;
            var value = peakValues.PeakValues.Average();
            microphone.MasterVolumeLevelScalar = (float)(Math.Sqrt(referenceLevel) / Math.Sqrt(value));
        }
        _view.NotifyCalibrated(microphones);
    }

    public void Analyze()
    {
        // すべてのマイクを取得する。
        using IMicrophones microphones = _microphonesProvider.Resolve();

        // キャプチャーを開始する。
        microphones.StartRecording();

        // 画面に入力レベルを通知する。
        _view.StartNotifyMasterPeakValue(microphones);

        Console.ReadLine();

        // 画面の入力レベル通知を停止する。
        _view.StopNotifyMasterPeakValue();

        // キャプチャーを停止する
        var peakValuesList = microphones.StopRecording().ToList();
    }
}