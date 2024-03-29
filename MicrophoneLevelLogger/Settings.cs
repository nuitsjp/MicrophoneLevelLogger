﻿namespace MicrophoneLevelLogger;

/// <summary>
/// MicrophoneLevelLoggerの各種設定
/// </summary>
public class Settings
{
    /// <summary>
    /// マイクの別名
    /// </summary>
    private readonly List<Alias> _aliases;
    /// <summary>
    /// 無効化されているマイク
    /// </summary>
    private readonly List<MicrophoneId> _disabledMicrophones;

    /// <summary>
    /// インスタンスを生成する。
    /// </summary>
    /// <param name="mediaPlayerHost"></param>
    /// <param name="recorderHost"></param>
    /// <param name="recordingSpan"></param>
    /// <param name="isEnableRemotePlaying"></param>
    /// <param name="isEnableRemoteRecording"></param>
    /// <param name="aliases"></param>
    /// <param name="disabledMicrophones"></param>
    /// <param name="selectedSpeakerId"></param>
    public Settings(
        string mediaPlayerHost,
        string recorderHost,
        TimeSpan recordingSpan,
        bool isEnableRemotePlaying,
        bool isEnableRemoteRecording, 
        IReadOnlyList<Alias> aliases, 
        IReadOnlyList<MicrophoneId> disabledMicrophones, 
        SpeakerId? selectedSpeakerId)
    {
        MediaPlayerHost = mediaPlayerHost;
        RecorderHost = recorderHost;
        RecordingSpan = recordingSpan;
        IsEnableRemotePlaying = isEnableRemotePlaying;
        IsEnableRemoteRecording = isEnableRemoteRecording;
        SelectedSpeakerId = selectedSpeakerId;
        _aliases = aliases.ToList();
        _disabledMicrophones = disabledMicrophones.ToList();
    }

    /// <summary>
    /// 録音時間
    /// </summary>
    public TimeSpan RecordingSpan { get; }
    /// <summary>
    /// リモート再生を有効とする
    /// </summary>
    public bool IsEnableRemotePlaying { get; }
    /// <summary>
    /// リモート再生ホスト
    /// </summary>
    public string MediaPlayerHost { get; }
    /// <summary>
    /// リモート録音を有効とする
    /// </summary>
    public bool IsEnableRemoteRecording { get; }
    /// <summary>
    /// リモート録音ホスト
    /// </summary>
    public string RecorderHost { get; }
    /// <summary>
    /// マイクの別名
    /// </summary>
    public IReadOnlyList<Alias> Aliases => _aliases;
    /// <summary>
    /// 無効化されたマイク
    /// </summary>
    public IReadOnlyList<MicrophoneId> DisabledMicrophones => _disabledMicrophones;
    public SpeakerId? SelectedSpeakerId { get; }

    /// <summary>
    /// 別名を更新する
    /// </summary>
    /// <param name="alias"></param>
    public void UpdateAlias(Alias alias)
    {
        _aliases.Remove(x => x.Id == alias.Id);
        _aliases.Add(alias);
    }

    /// <summary>
    /// 別名を削除する
    /// </summary>
    /// <param name="alias"></param>
    public void RemoveAlias(Alias alias)
    {
        _aliases.Remove(alias);
    }

    /// <summary>
    /// マイクを無効化する
    /// </summary>
    /// <param name="id"></param>
    public void DisableMicrophone(MicrophoneId id)
    {
        if (_disabledMicrophones.Contains(id))
        {
            return;
        }
        _disabledMicrophones.Add(id);
    }

    /// <summary>
    /// マイクを有効化する
    /// </summary>
    /// <param name="id"></param>
    public void EnableMicrophone(MicrophoneId id)
    {
        if (_disabledMicrophones.Contains(id))
        {
            _disabledMicrophones.Remove(id);
        }
    }

}