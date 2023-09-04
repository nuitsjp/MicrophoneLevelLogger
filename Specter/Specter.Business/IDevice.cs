using System;
using System.ComponentModel;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Specter.Business;

/// <summary>
/// マイク
/// </summary>
public interface IDevice : IDisposable, INotifyPropertyChanged
{
    /// <summary>
    /// ID
    /// </summary>
    DeviceId Id { get; }
    /// <summary>
    /// DataFlow
    /// </summary>
    DataFlow DataFlow { get; }
    /// <summary>
    /// 名称
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Windows上の名称
    /// </summary>
    string SystemName { get; }

    /// <summary>
    /// 計測するか、しないか取得する。
    /// </summary>
    bool Measure { get; set; }
    /// <summary>
    /// 入力レベル
    /// </summary>
    VolumeLevel VolumeLevel { get; set; }
    
    IObservable<WaveInEventArgs>　WaveInput { get; }
    IObservable<Decibel>　InputLevel { get; }

    void StartMonitoring(WaveFormat waveFormat, RefreshRate refreshRate);
    void StopMonitoring();
}