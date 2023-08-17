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
    private NetworkStream _networkStream;
    private IDisposable _disposable;

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
        _networkStream
            .ConvertStreamToReactive()
            .Subscribe(x =>
            {
                // サーバーからのコマンドを受信し、音声データの送信を開始・停止する。
                var command = x.Bytes[0];
                if (command == RemoteDevice.Start)
                {
                    _source.Subscribe(x =>
                    {
                        _networkStream.Write(x.Buffer, 0, x.BytesRecorded);
                    }).AddTo(_compositeDisposable);
                }
                else
                {
                    _disposable.Dispose();
                }
            });
    }

    public void Dispose()
    {
        _compositeDisposable.Dispose();
    }
}