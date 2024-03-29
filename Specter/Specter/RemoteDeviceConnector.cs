﻿using System.Net.Sockets;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;

namespace Specter;

/// <summary>
/// ローカルの音声データを、リモートのサーバーに接続するためのコネクター。
/// </summary>
public class RemoteDeviceConnector : IDisposable
{
    private readonly IRenderDevice _device;
    private readonly TcpClient _tcpClient;
    private readonly string _address;
    private NetworkStream? _networkStream;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly CompositeDisposable _compositeDisposable = new();

    public RemoteDeviceConnector(
        string address,
        IRenderDevice device)
    {
        _address = address;
        _device = device;
        _tcpClient = new TcpClient().AddTo(_compositeDisposable);
    }

    public void Connect()
    {
        _tcpClient.Connect(_address, RemoteDeviceInterface.ServerPort.AsPrimitive());
        _networkStream = _tcpClient.GetStream().AddTo(_compositeDisposable);
        _device
            .WaveInput
            .Subscribe(x =>
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
        _networkStream.ConvertStreamToReactive()
            .Subscribe(value =>
            {
                var command = value.Bytes[0];
                switch (command)
                {
                    case RemoteDevice.StartCommand:
                        _cancellationTokenSource = new();
                        _device.PlayLooping(_cancellationTokenSource.Token);
                        break;
                    case RemoteDevice.StopCommand:
                        _cancellationTokenSource?.Cancel();
                        _cancellationTokenSource = null;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            })
            .AddTo(_compositeDisposable);
    }

    public void Dispose()
    {
        _tcpClient.Close();
        _networkStream = null;
        _compositeDisposable.Dispose();
    }
}