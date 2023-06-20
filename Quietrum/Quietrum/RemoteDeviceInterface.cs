using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CommunityToolkit.Mvvm.ComponentModel;
using Reactive.Bindings;

namespace Quietrum;

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
                _devices.Add(new RemoteDevice(tcpClient));
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
