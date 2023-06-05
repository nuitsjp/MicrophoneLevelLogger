using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kamishibai;
using Reactive.Bindings;
using Reactive.Bindings.Disposables;
using Reactive.Bindings.Extensions;
using ScottPlot;

namespace Quietrum.ViewModel;

[Navigate]
// ReSharper disable once ClassNeverInstantiated.Global
public partial class MonitoringPageViewModel : ObservableObject, INavigatedAsyncAware, IDisposable
{
    private readonly CompositeDisposable _compositeDisposable = new();
    public RecordingConfig RecordingConfig { get; } = RecordingConfig.Default;

    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    [ObservableProperty] private bool _monitor = true;
    [ObservableProperty] private bool _record;
    [ObservableProperty] private string _recordName = string.Empty;
    [ObservableProperty] private TimeSpan _elapsed = TimeSpan.Zero;
    [ObservableProperty] private IList<DeviceViewModel> _devices = new List<DeviceViewModel>();
    
    public MonitoringPageViewModel(
        [Inject] IAudioInterfaceProvider audioInterfaceProvider)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        this.ObserveProperty(x => x.Monitor)
            .Skip(1)
            .Subscribe(OnMonitor)
            .AddTo(_compositeDisposable);
        this.ObserveProperty(x => x.Record)
            .Skip(1)
            .Subscribe(OnRecord)
            .AddTo(_compositeDisposable);
    }

    private void OnMonitor(bool monitor)
    {
        if (monitor)
        {
            Devices
                .Where(x => x.Measure)
                .ToList()
                .ForEach(x => x.StartMonitoring());
        }
        else
        {
            Devices
                .Where(x => x.Measure)
                .ToList()
                .ForEach(x => x.StopMonitoring());
        }
    }

    private void OnRecord(bool record)
    {
        if (record)
        {
            StartRecording();
        }
        else
        {
            StopRecording();
        }
    }

    private void StartRecording()
    {
        const string recordNameFormat = "yyyy.MM.dd-HH.mm.ss";
        do
        {
            RecordName = DateTime.Now.ToString(recordNameFormat);
        } while (Directory.Exists(RecordName));

        var directory = new DirectoryInfo(RecordName);
        directory.Create();

        Devices
            .Where(x => x.Measure)
            .ToList()
            .ForEach(x => x.StartRecording(directory));
    }

    private void StopRecording()
    {
        Devices
            .Where(x => x.Measure)
            .ToList()
            .ForEach(x => x.StopRecording());
    }

    public async Task OnNavigatedAsync(PostForwardEventArgs args)
    {
        var audioInterface = await _audioInterfaceProvider.ResolveAsync();
        audioInterface
            .ObserveProperty(x => x.Microphones)
            .Subscribe(microphones =>
            {
                List<DeviceViewModel> newViewModels = new(Devices);
                // 接続されたIMicrophoneを追加する。
                newViewModels.AddRange(
                    microphones
                        .Where(x => newViewModels.NotContains(viewModel => viewModel.Id == x.Id))
                        .Select(x =>
                        {
                            var microphone = new DeviceViewModel(x, RecordingConfig);
                            microphone.PropertyChanged += MicrophoneOnPropertyChanged;
                            if (microphone.Measure)
                            {
                                microphone.StartMonitoring();
                            }
                            return microphone;
                        }));
                // 除去されたIMicrophoneを削除する
                newViewModels.Where(x => microphones.NotContains(microphone => microphone.Id == x.Id))
                    .ToList()
                    .ForEach(viewModel =>
                    {
                        viewModel.PropertyChanged -= MicrophoneOnPropertyChanged;
                        newViewModels.Remove(viewModel);
                    });
                Devices = newViewModels;
            });
    }

    private void MicrophoneOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Devices));
    }

    public void Dispose()
    {
        Devices.Dispose();
        Devices.Dispose();
        _compositeDisposable.Dispose();
    }
}