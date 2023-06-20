using System.Net.Sockets;
using NAudio.Wave;

namespace Quietrum;

public class RemoteDeviceConnector
{
    private readonly IObservable<WaveInEventArgs> _source;
    private readonly TcpClient _tcpClient;
    private readonly string _address;

    public RemoteDeviceConnector(
        string address,
        IObservable<WaveInEventArgs> source)
    {
        _address = address;
        _source = source;
        _tcpClient = new TcpClient();
    }

    public void Connect()
    {
        _tcpClient.Connect(_address, RemoteDeviceInterface.ServerPort.AsPrimitive());
    }
}