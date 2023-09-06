using NAudio.Wave;
using Specter.Business;

namespace Specter.Repository;

public class AudioRecorder : IAudioRecorder
{
    private static readonly DirectoryInfo RootDirectory = new("Record");

    private DirectoryInfo? _directoryInfo;
    private readonly IDevice _targetDevice;
    private readonly Direction _direction;
    private readonly List<IDevice> _monitoringDevices;
    private readonly List<IDeviceRecorder> _deviceRecorders = new();
    private readonly WaveFormat _waveFormat;
    private readonly IAudioRecordRepository _audioRecordRepository;

    private DateTime _startDateTime;

    public AudioRecorder(
        IDevice targetDevice, 
        Direction direction, 
        IEnumerable<IDevice> monitoringDevices,
        WaveFormat waveFormat, 
        IAudioRecordRepository audioRecordRepository)
    {
        _targetDevice = targetDevice;
        _monitoringDevices = monitoringDevices.ToList();
        _direction = direction;
        _waveFormat = waveFormat;
        _audioRecordRepository = audioRecordRepository;
    }

    public void Start()
    {
        _startDateTime = DateTime.Now;
        _directoryInfo = new DirectoryInfo(Path.Combine(RootDirectory.FullName, $"{DateTime.Now:yyyy.MM.dd-HH.mm.ss}_{_targetDevice.Name}_{_direction}"));
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

        await _audioRecordRepository.SaveAsync(audioRecord);
        
        return audioRecord;
    }
}