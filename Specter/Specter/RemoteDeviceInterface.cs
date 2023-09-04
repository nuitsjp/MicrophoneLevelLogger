using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Reactive.Bindings;

namespace Specter.Business;

public partial class RemoteDeviceInterface : ObservableObject, IDeviceInterface, IDisposable
{
    public static readonly Port ServerPort = new(9876);

    private readonly TcpListener _tcpListener = new(IPAddress.Any, ServerPort.AsPrimitive());
    
    private readonly Task _task;

    private readonly ReactiveCollection<IDevice> _devices = new();

    public RemoteDeviceInterface()
    {
        _task = new Task(OnListening);
        Devices = _devices
            .ToReadOnlyReactiveCollection(scheduler: CurrentThreadScheduler.Instance);
    }

    public ReadOnlyReactiveCollection<IDevice> Devices { get; }

    public Task ActivateAsync()
    {
        _task.Start();
        return Task.CompletedTask;
    }

    private void OnListening()
    {
        _tcpListener.Start();

        try
        {
            while (true)
            {
                var tcpClient = _tcpListener.AcceptTcpClient();
                var remoteDevice = new RemoteDevice(tcpClient); 
                remoteDevice.Disconnected += OnDisconnected;
                _devices.Add(remoteDevice);
            }
        }
        catch
        {
            // stopped
        }
    }

    private void OnDisconnected(object? sender, EventArgs e)
    {
        if(sender is null) return;
        
        RemoteDevice remoteDevice = (RemoteDevice)sender;
        _devices.Remove(remoteDevice);
        remoteDevice.Disconnected -= OnDisconnected;
    }

    public void Dispose()
    {
        _tcpListener.Stop();
        _task.Dispose();
    }

}
