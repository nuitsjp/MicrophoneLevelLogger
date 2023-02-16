﻿using System.CodeDom;
using FftSharp.Windows;

namespace MicrophoneLevelLogger;

public class AudioInterfaceLogger : IAudioInterfaceLogger
{
    private static readonly DirectoryInfo RootDirectory = new("Record");
    private readonly StreamWriter _maxDecibelLogger;

    public AudioInterfaceLogger(
        IAudioInterface audioInterface, 
        string? recordName = null)
    {
        DirectoryInfo? directoryInfo =
            recordName is not null
                ? new DirectoryInfo(Path.Join(RootDirectory.FullName, $"{DateTime.Now:yyyy-mm-dd_hhMMss}_{recordName}"))
                : null;
        directoryInfo?.Create();
        MicrophoneLoggers = audioInterface
            .Microphones
            .Select(x => (IMicrophoneLogger)new MicrophoneLogger(x, directoryInfo))
            .ToList();
        _maxDecibelLogger = directoryInfo is not null
            ? File.CreateText(Path.Combine(directoryInfo.FullName, "detail.csv"))
            : StreamWriter.Null;
    }

    public IReadOnlyList<IMicrophoneLogger> MicrophoneLoggers { get; }

    public async Task StartAsync(CancellationToken token)
    {
        // すべてのマイクのログインぐを開始する。
        await MicrophoneLoggers.ForEachAsync(x => x.StartAsync(token));

        // 非同期でロギングを実施する。
        var task = Task.Run(async () => await LoggingAsync(token), token);

        // キャンセル処理ハンドラーを登録する。
        token.Register(() =>
        {
            // ReSharper disable once MethodSupportsCancellation
            task.Wait();
        });
    }

    private async Task LoggingAsync(CancellationToken token)
    {
        // CSVのヘッダーを出力する。
        await WriteHeaderAsync();

        // キャンセルされるまで繰り返す。
        while (token.IsCancellationRequested is false)
        {
            // レコードを出力する。
            await WriteRecordAsync();
            try
            {
                // サンプリング間隔待機する。
                await Task.Delay(IMicrophone.SamplingMilliseconds, token);
            }
            catch (TaskCanceledException)
            {
            }
        }

        // ストリームをフラッシュして破棄する。
        await _maxDecibelLogger.FlushAsync();
    }

    private async Task WriteRecordAsync()
    {
        await _maxDecibelLogger.WriteAsync($"{DateTime.Now:yyyy/MM/dd hh:mm:ss.fff}");
        foreach (var microphoneLogger in MicrophoneLoggers)
        {
            await _maxDecibelLogger.WriteAsync(",");
            _maxDecibelLogger.Write(microphoneLogger.Max);
        }

        await _maxDecibelLogger.WriteLineAsync();
    }

    private async Task WriteHeaderAsync()
    {
        await _maxDecibelLogger.WriteAsync("時刻");
        foreach (var microphoneLogger in MicrophoneLoggers)
        {
            await _maxDecibelLogger.WriteAsync(",");
            await _maxDecibelLogger.WriteAsync(microphoneLogger.Microphone.Name);
        }

        await _maxDecibelLogger.WriteLineAsync();
    }

    public void Dispose()
    {
        foreach (var logger in MicrophoneLoggers)
        {
            logger.DisposeQuiet();
        }
        _maxDecibelLogger.Dispose();
    }
}