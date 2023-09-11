using System.Reactive.Disposables;
using System.Text.Json;
using NAudio.Wave;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Specter.Repository;

public class AudioRecordInterface : IAudioRecordInterface, IDisposable
{
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

    public bool Activated { get; private set; }

    public async Task ActivateAsync()
    {
        if (Activated) return;

        var records = await LoadAsync();
        foreach (var record in records)
        {
            _audioRecords.Add(record);
        }

        Activated = true;
    }

    public IAudioRecorder BeginRecording(
        IDevice targetDevice,
        Direction direction,
        IEnumerable<IDevice> monitoringDevices,
        WaveFormat waveFormat)
    {
        return new AudioRecorder(
            targetDevice,
            direction,
            monitoringDevices,
            waveFormat,
            this);
    }

    public static string GetAudioRecordPath(AudioRecord audioRecord)
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

    private class AudioRecorder : IAudioRecorder
    {
        private static readonly DirectoryInfo RootDirectory = new("Record");

        private DirectoryInfo? _directoryInfo;
        private readonly IDevice _targetDevice;
        private readonly Direction _direction;
        private readonly List<IDevice> _monitoringDevices;
        private readonly List<IDeviceRecorder> _deviceRecorders = new();
        private readonly WaveFormat _waveFormat;
        private readonly IAudioRecordInterface _audioRecordInterface;

        private DateTime _startDateTime;

        public AudioRecorder(
            IDevice targetDevice,
            Direction direction,
            IEnumerable<IDevice> monitoringDevices,
            WaveFormat waveFormat,
            IAudioRecordInterface audioRecordInterface)
        {
            _targetDevice = targetDevice;
            _monitoringDevices = monitoringDevices.ToList();
            _direction = direction;
            _waveFormat = waveFormat;
            _audioRecordInterface = audioRecordInterface;
        }

        public void Start()
        {
            _startDateTime = DateTime.Now;
            _directoryInfo = new DirectoryInfo(Path.Combine(RootDirectory.FullName,
                $"{DateTime.Now:yyyy.MM.dd-HH.mm.ss}_{_targetDevice.Name}_{_direction}"));
            _directoryInfo.Create();
            _deviceRecorders.AddRange(
                _monitoringDevices
                    .Select(x => new DeviceRecorder(_directoryInfo, x, _waveFormat)));
            foreach (var deviceRecorder in _deviceRecorders)
            {
                deviceRecorder.Start();
            }
        }

        public async Task<AudioRecord> StopAsync()
        {
            List<DeviceRecord> deviceRecords = new();
            foreach (var deviceRecorder in _deviceRecorders)
            {
                deviceRecords.Add(deviceRecorder.Stop());
            }

            var audioRecord = new AudioRecord(
                _targetDevice.Id,
                _direction,
                _startDateTime,
                DateTime.Now,
                deviceRecords.ToArray());

            await _audioRecordInterface.SaveAsync(audioRecord);

            return audioRecord;
        }
    }
}