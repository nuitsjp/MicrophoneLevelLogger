using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Kamishibai;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ScottPlot;

namespace Quietrum.ViewModel;

// ReSharper disable once ClassNeverInstantiated.Global
public partial class MainWindowViewModel : ObservableObject, INavigatedAsyncAware, IDisposable
{
    public RecordingConfig RecordingConfig { get; } = RecordingConfig.Default;

    private readonly IAudioInterfaceProvider _audioInterfaceProvider;
    [ObservableProperty]
    private TimeSpan _elapsed = TimeSpan.Zero;

    private readonly ReactivePropertySlim<IList<MicrophoneViewModel>> _microphones = new(new List<MicrophoneViewModel>());
    public ReadOnlyReactivePropertySlim<IList<MicrophoneViewModel>> Microphones { get; }
    
    public MainWindowViewModel(IAudioInterfaceProvider audioInterfaceProvider)
    {
        _audioInterfaceProvider = audioInterfaceProvider;
        Microphones = _microphones.ToReadOnlyReactivePropertySlim()!;
    }

    public async Task OnNavigatedAsync(PostForwardEventArgs args)
    {
        var audioInterface = await _audioInterfaceProvider.ResolveAsync();
        audioInterface
            .ObserveProperty(x => x.Microphones)
            .Subscribe(microphones =>
            {
                List<MicrophoneViewModel> newViewModels = new(Microphones.Value);
                // 接続されたIMicrophoneを追加する。
                newViewModels.AddRange(
                    microphones
                        .Where(x => newViewModels.NotContains(viewModel => viewModel.Id == x.Id))
                        .Select(x =>
                        {
                            var microphone = new MicrophoneViewModel(x, RecordingConfig);
                            microphone.StartRecording();
                            return microphone;
                        }));
                // 除去されたIMicrophoneを削除する
                newViewModels.Where(x => microphones.NotContains(microphone => microphone.Id == x.Id))
                    .ToList()
                    .ForEach(viewModel =>
                    {
                        newViewModels.Remove(viewModel);
                    });
                _microphones.Value = newViewModels;
            });
    }

    public void Dispose()
    {
        Microphones.Value.Dispose();
        Microphones.Dispose();
    }
}