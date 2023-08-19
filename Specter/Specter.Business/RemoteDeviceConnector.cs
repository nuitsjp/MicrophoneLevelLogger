using System.Net.Sockets;
using NAudio.Wave;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;

namespace Specter.Business;

/// <summary>
/// ローカルの音声データを、リモートのサーバーに接続するためのコネクター。
/// </summary>
public class RemoteDeviceConnector : IDisposable
{
    private readonly IObservable<WaveInEventArgs> _source;
    private readonly TcpClient _tcpClient;
    private readonly string _address;
    private NetworkStream? _networkStream;
    private readonly CompositeDisposable _compositeDisposable = new();

    public RemoteDeviceConnector(
        string address,
        IObservable<WaveInEventArgs> source)
    {
        _address = address;
        _source = source;
        _tcpClient = new TcpClient().AddTo(_compositeDisposable);
    }

    public void Connect()
    {
        _tcpClient.Connect(_address, RemoteDeviceInterface.ServerPort.AsPrimitive());
        _networkStream = _tcpClient.GetStream().AddTo(_compositeDisposable);
        _source.Subscribe(x =>
        {
            try
            {
                _networkStream?.WriteAsync(x.Buffer, 0, x.BytesRecorded);
            }
            catch
            {
                // ignore
            }
        }).AddTo(_compositeDisposable);
    }

    public void Dispose()
    {
        _tcpClient.Close();
        _networkStream = null;
        _compositeDisposable.Dispose();
    }
}