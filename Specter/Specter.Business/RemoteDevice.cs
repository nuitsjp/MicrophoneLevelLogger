using System.Net.Sockets;
using System.Reactive.Subjects;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Specter.Business;

public class RemoteDevice : ObservableObject, IRenderDevice
{
    public const byte StartCommand = 0;
    public const byte StopCommand = 1;
    
    public event EventHandler<EventArgs>? Disconnected;
    
    private readonly TcpClient _tcpClient;
    private readonly NetworkStream _networkStream;
    private readonly Subject<WaveInEventArgs> _subject = new();
    private readonly Task _backgroundTask;
    private bool _recording;

    public RemoteDevice(TcpClient tcpClient)
    {
        _tcpClient = tcpClient;
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
    public IObservable<WaveInEventArgs> StartMonitoring(WaveFormat waveFormat, TimeSpan bufferSpan)
    {
        _recording = true;
        return _subject;
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

    public Task PlayLoopingAsync(CancellationToken token)
    {

        // 終了処理を登録する。
        token.Register(() =>
        {
            _networkStream.WriteByte(StopCommand);
        });

        _networkStream.WriteByte(StartCommand);
        
        return Task.CompletedTask;
    }

    private void Close()
    {
        Disconnected?.Invoke(this, EventArgs.Empty);
    }
}