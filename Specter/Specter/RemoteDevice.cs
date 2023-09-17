using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Specter;

public class RemoteDevice : ObservableObject, IRenderDevice
{
    public const byte StartCommand = 0;
    public const byte StopCommand = 1;
    
    public event EventHandler<EventArgs>? Disconnected;
    
    private readonly TcpClient _tcpClient;
    private readonly IFastFourierTransformSettings _fastFourierTransformSettings;
    private readonly NetworkStream _networkStream;
    private readonly Subject<WaveInEventArgs> _subject = new();
    private readonly Task _backgroundTask;
    private bool _recording;
    private IDisposable? _waveInUnsubscribe;

    public RemoteDevice(
        TcpClient tcpClient, 
        IFastFourierTransformSettings fastFourierTransformSettings)
    {
        _tcpClient = tcpClient;
        _fastFourierTransformSettings = fastFourierTransformSettings;
        _networkStream = tcpClient.GetStream();
        
        _backgroundTask = ReadAsync();
        
        Id = new DeviceId(_tcpClient.Client.RemoteEndPoint!.ToString()!);
        SystemName = Id.AsPrimitive();
        Name = SystemName;
        VolumeLevel = new VolumeLevel(1f);
    }

    public void Dispose()
    {
        _tcpClient.Close();
        _tcpClient.Dispose();
    }

    public DeviceId Id { get; }
    public DataFlow DataFlow => DataFlow.Render;
    public string Name { get; set; }
    public string SystemName { get; }
    public bool Measure { get; set; }
    public VolumeLevel VolumeLevel { get; set; }
    public IObservable<WaveInEventArgs> WaveInput => _subject.AsObservable();
    private readonly Subject<Decibel> _inputLevel = new();
    public IObservable<Decibel> InputLevel => _inputLevel.AsObservable();

    public void StartMonitoring(WaveFormat waveFormat, RefreshRate refreshRate)
    {
        _recording = true;
        _waveInUnsubscribe = new WaveInToInputLevelObservable(this, waveFormat, refreshRate, _fastFourierTransformSettings)
            .Subscribe(x => _inputLevel.OnNext(x));
    }

    private async Task ReadAsync()
    {
        try
        {
            var bytes = new byte[256];
            while (true)
            {
                var length = await _networkStream.ReadAsync(bytes, 0, bytes.Length);
                if (length == 0)
                {
                    _recording = false;
                    break;
                }
                if (_recording)
                {
                    _subject.OnNext(new WaveInEventArgs(bytes, length));
                }
            }
        }
        catch
        {
            // TODO
        }
        finally
        {
            Close();
        }
    }

    public void StopMonitoring()
    {
        _recording = false;
    }

    public void PlayLooping(CancellationToken token)
    {

        // 終了処理を登録する。
        token.Register(() =>
        {
            _networkStream.WriteByte(StopCommand);
        });

        _networkStream.WriteByte(StartCommand);
    }

    private void Close()
    {
        Disconnected?.Invoke(this, EventArgs.Empty);
    }
}