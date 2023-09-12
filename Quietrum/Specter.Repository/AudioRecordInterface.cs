using System.Reactive.Disposables;
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
        IEnumerable<IDevice> monitoringDevices,
        WaveFormat waveFormat)
    {
        var startDateTime = DateTime.Now;
        var directoryInfo = new DirectoryInfo(Path.Combine(RootDirectory,
            $"{startDateTime:yyyy.MM.dd-HH.mm.ss}_{targetDevice.Name}_{direction}"));
        directoryInfo.Create();

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
                startDateTime,
                DateTime.Now,
                deviceRecords.ToArray());

            await SaveAsync(audioRecord);
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
        return Path.Combine(
            RootDirectory,
            $"{audioRecord.StartTime:yyyy.MM.dd-HH.mm.ss}_{targetDevice.Name}_{audioRecord.Direction}");
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