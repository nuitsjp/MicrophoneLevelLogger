﻿using System.Reactive.Disposables;
using System.Text.Json;
using NAudio.Wave;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Specter.Repository;

public class AudioRecordInterface : IAudioRecordInterface, IDisposable
{
    private bool _activated;

    private static readonly string RootDirectory = "Record";
    private static readonly string AudioRecordFile = "AudioRecord.json";

    private readonly CompositeDisposable _compositeDisposable = new();
    private readonly ReactiveCollection<AudioRecord> _audioRecords = new();
    public ReadOnlyReactiveCollection<AudioRecord> AudioRecords { get; }

    public AudioRecordInterface()
    {
        _audioRecords.AddTo(_compositeDisposable);

        AudioRecords = _audioRecords.ToReadOnlyReactiveCollection();
    }

    public async Task ActivateAsync()
    {
        if (_activated)
        {
            throw new InvalidOperationException("Already Activated.");
        }

        var records = await LoadAsync();
        foreach (var record in records)
        {
            _audioRecords.Add(record);
        }

        _activated = true;
    }

    public IAudioRecording BeginRecording(
        IDevice targetDevice,
        Direction direction,
        BuzzState buzzState,
        VoiceState voiceState,
        IEnumerable<IDevice> monitoringDevices,
        IRenderDevice? playbackDevice,
        WaveFormat waveFormat)
    {
        var startDateTime = DateTime.Now;
        var directoryInfo = new DirectoryInfo(GetAudioRecordPath(startDateTime, targetDevice.Name, direction, buzzState, voiceState));
        directoryInfo.Create();

        var playBackCancellationTokenSource = new CancellationTokenSource();
        if (buzzState == BuzzState.With)
        {
            playbackDevice?.PlayLooping(playBackCancellationTokenSource.Token);
        }

        var deviceRecorders = monitoringDevices
            .Select(device =>
            {
                var waveFile = Path.Combine(directoryInfo.FullName, $"{device.Name}.wav");
                var inputLevelFile = File.Create(Path.Combine(directoryInfo.FullName, $"{device.Name}.ilv"));

                return new DeviceRecorder(
                    device, 
                    new WaveFileWriter(waveFile, waveFormat), 
                    new BinaryWriter(inputLevelFile));
            })
            .ToList();
        deviceRecorders.ForEach(x => x.Start());
        

        var audioRecorder = new AudioRecording();

        audioRecorder.EndRecordingCommand.Subscribe(async () =>
        {
            List<DeviceRecord> deviceRecords = new();
            foreach (var deviceRecorder in deviceRecorders)
            {
                deviceRecords.Add(deviceRecorder.Stop());
            }

            var audioRecord = new AudioRecord(
                targetDevice.Id,
                direction,
                buzzState,
                voiceState,
                startDateTime,
                DateTime.Now,
                deviceRecords.ToArray());

            await SaveAsync(audioRecord);
            await playBackCancellationTokenSource.CancelAsync();
        });
        
        return audioRecorder;
    }

    public IEnumerable<Decibel> ReadInputLevels(
        AudioRecord audioRecord,
        DeviceRecord deviceRecord)
    {
        var file = Path.Combine(
            GetAudioRecordPath(audioRecord),
            $"{deviceRecord.Name}.ilv");

        var reader = new BinaryReader(File.OpenRead(file));
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            yield return new Decibel(reader.ReadDouble());
        }
    }

    public void DeleteAudioRecord(AudioRecord audioRecord)
    {
        Directory.Delete(GetAudioRecordPath(audioRecord), true);
        _audioRecords.Remove(audioRecord);
    }

    private static string GetAudioRecordPath(AudioRecord audioRecord)
    {
        var targetDevice = audioRecord.DeviceRecords.Single(x => x.Id == audioRecord.TargetDeviceId);
        return GetAudioRecordPath(audioRecord.StartTime, targetDevice.Name, audioRecord.Direction, audioRecord.BuzzState, audioRecord.VoiceState);
    }

    private static string GetAudioRecordPath(DateTime startTime, string deviceName, Direction direction, BuzzState buzzState, VoiceState voiceState)
    {
        string stateMessage;
        if (buzzState == BuzzState.Without && voiceState == VoiceState.Without)
        {
            stateMessage = string.Empty;
        }
        else if (buzzState == BuzzState.With && voiceState == VoiceState.Without)
        {
            stateMessage = "_with_Buzz";
        }
        else if (buzzState == BuzzState.Without && voiceState == VoiceState.With)
        {
            stateMessage = "_with_Voice";
        }
        else
        {
            stateMessage = "_with_Voice_and_Buzz";
        }
        return Path.Combine(
            RootDirectory,
            $"{startTime:yyyy.MM.dd-HH.mm.ss}_{deviceName}_{direction}{stateMessage}");
    }

    public async Task SaveAsync(AudioRecord audioRecord)
    {
        var filePath = Path.Combine(
            GetAudioRecordPath(audioRecord),
            AudioRecordFile);

        var directoryName = Path.GetDirectoryName(filePath)!;
        if (!Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);

        await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(stream, audioRecord, JsonEnvironments.Options);
        _audioRecords.Add(audioRecord);
    }

    public async Task<IEnumerable<AudioRecord>> LoadAsync()
    {
        if (Directory.Exists(RootDirectory) is false)
            Directory.CreateDirectory(RootDirectory);

        var directories = Directory.GetDirectories(RootDirectory);

        List<AudioRecord> records = new();
        foreach (var directory in directories)
        {
            var file = Path.Combine(directory, AudioRecordFile);
            if (File.Exists(file) is false)
                continue;

            records.Add(await LoadAsync(file));
        }

        return records;
    }

    private async Task<AudioRecord> LoadAsync(string file)
    {
        await using var stream = new FileStream(file, FileMode.Open, FileAccess.Read);
        return (await JsonSerializer.DeserializeAsync<AudioRecord>(stream, JsonEnvironments.Options))!;
    }

    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }

    private class AudioRecording : IAudioRecording
    {
        public AsyncReactiveCommand EndRecordingCommand { get; } = new();

        public async Task EndRecordingAsync()
        {
            await EndRecordingCommand.ExecuteAsync();
        }
    }
}