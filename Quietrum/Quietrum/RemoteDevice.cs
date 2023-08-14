using System.ComponentModel;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Quietrum;

public class RemoteDevice : ObservableObject, IDevice
{
    public static readonly byte Start = 1;
    public static readonly byte Stop = 0;

    public event EventHandler<EventArgs> Disconnected;
    
    private readonly TcpClient _tcpClient;
    private readonly NetworkStream _networkStream;
    private readonly Subject<WaveInEventArgs> _subject = new();
    private readonly Task _backgroundTask;

    public RemoteDevice(TcpClient tcpClient)
    {
        _tcpClient = tcpClient;
        _networkStream = tcpClient.GetStream();
        
        _backgroundTask = Task.Run(() =>
        {
            try
            {
                int length;
                var bytes = new byte[256];
                while ((length = _networkStream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    _subject.OnNext(new WaveInEventArgs(bytes, length));
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
        });
        
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
    public DataFlow DataFlow => DataFlow.Capture;
    public string Name { get; set; }
    public string SystemName { get; }
    public bool Measure { get; set; }
    public VolumeLevel VolumeLevel { get; set; }
    public IObservable<WaveInEventArgs> StartRecording(WaveFormat waveFormat, TimeSpan bufferSpan)
    {
        byte[] command = {Start};
        _networkStream.Write(command);
        return _subject;
    }

    public void StopRecording()
    {
        byte[] command = {Stop};
        _networkStream.Write(command);
        Close();
    }

    private void Close()
    {
        Disconnected?.Invoke(this, EventArgs.Empty);
    }
}