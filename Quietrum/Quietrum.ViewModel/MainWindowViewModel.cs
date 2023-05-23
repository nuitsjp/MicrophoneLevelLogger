using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using Kamishibai;
using ScottPlot;

namespace Quietrum.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject, INavigatedAware
    {
        private readonly IAudioInterface _audioInterface;
        [ObservableProperty]
        private TimeSpan _elapsed = TimeSpan.Zero;

        public double[] LiveData { get; } = new double[400];

        private readonly DataGen.Electrocardiogram _ecg = new();
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        private readonly Timer _updateDataTimer;

        public TimeSpan Period { get; } = TimeSpan.FromMilliseconds(50);

        public MainWindowViewModel(IAudioInterface audioInterface)
        {
            _audioInterface = audioInterface;
            _updateDataTimer = new Timer(_ => UpdateData(), null, Timeout.Infinite, Timeout.Infinite);
        }
        
        void UpdateData()
        {
            // "scroll" the whole chart to the left
            Array.Copy(LiveData, 1, LiveData, 0, LiveData.Length - 1);

            // place the newest data point at the end
            var nextValue = _ecg.GetVoltage(_stopwatch.Elapsed.TotalSeconds);
            LiveData[^1] = nextValue;
            Elapsed = _stopwatch.Elapsed;
        }

        public void OnNavigated(PostForwardEventArgs args)
        {
            var microphones = _audioInterface.GetMicrophones();
            var microphone = microphones.First();
            _updateDataTimer.Change(TimeSpan.Zero, Period);
        }
    }
}