using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Quietrum;

public partial class RemoteDeviceServer : ObservableObject, IRemoteDeviceServer, IDisposable
{
    public static readonly Port ServerPort = new(9876);

    private readonly TcpListener _tcpListener = new(IPAddress.Any, ServerPort.AsPrimitive());
    
    private readonly Task _task;
    [ObservableProperty] private IReadOnlyList<RemoteDevice> _remoteDevices = new List<RemoteDevice>();

    public RemoteDeviceServer()
    {
        _task = new Task(OnListening);
    }

    public event EventHandler<EventArgs>? RemoteDevicesChanged;

    public void Activate()
    {
        _task.Start();
    }


    private void OnListening()
    {
        _tcpListener.Start();

        try
        {
            while (true)
            {
                var tcpClient = _tcpListener.AcceptTcpClient();
                var newDevices = RemoteDevices.ToList();
                newDevices.Add(new RemoteDevice(tcpClient));
                RemoteDevices = newDevices;
                RemoteDevicesChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        catch
        {
            // stopped
        }
    }

    public void Dispose()
    {
        _tcpListener.Stop();
        _task.Dispose();
    }
}

public interface IRemoteDeviceServer
{
    public event EventHandler<EventArgs> RemoteDevicesChanged;
    
    public IReadOnlyList<RemoteDevice> RemoteDevices { get; }

    public void Activate();
}