using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Reactive.Bindings.Extensions;
using ScottPlot;
using Specter.ViewModel;

namespace Specter.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MonitoringPage
    {
        private static readonly TimeSpan DisplayWidth = TimeSpan.FromSeconds(20);
        
        private readonly DispatcherTimer _renderTimer = new()
        {
            Interval = TimeSpan.FromMilliseconds(10)
        };

        public MonitoringPage()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private MonitoringPageViewModel ViewModel => (MonitoringPageViewModel)DataContext;

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // plot the data array only once
            ViewModel.ObserveProperty(x => x.Devices).Subscribe(microphones =>
            {
                Dispatcher.Invoke(() =>
                {
                    WpfPlot1.Plot.Clear();
                    foreach (var microphone in microphones.Where(x => x.Measure))
                    {
                        WpfPlot1.Plot.AddSignal(microphone.LiveData, label:microphone.Name);
                    }
                    
                    WpfPlot1.Plot.AxisAutoX(margin: 0);

                    RecordingConfig config = ViewModel.RecordingConfig;
                    // データの全長から、デフォルトの表示幅分を引いた値をデフォルトのx軸の最小値とする
                    var xMin =
                        // データの全長
                        config.RecordingLength
                        // 表示時間を表示間隔で割ることで、表示幅を計算する
                        - (int)(DisplayWidth / ViewModel.RecordingConfig.RefreshRate.Interval);
                    WpfPlot1.Plot.SetAxisLimits(
                        xMin: xMin, xMax: config.RecordingLength,
                        yMin: -90, yMax: 0);
                    WpfPlot1.Plot.XAxis.SetBoundary(0, config.RecordingLength);
                    WpfPlot1.Plot.YAxis.SetBoundary(-90, 0);
                    WpfPlot1.Plot.XAxis.TickLabelFormat(x => $"{(((config.RecordingLength - x) * -1 * ViewModel.RecordingConfig.RefreshRate.Interval.TotalMilliseconds) / 1000d):#0.0[s]}");
                    WpfPlot1.Configuration.LockVerticalAxis = true;
                    WpfPlot1.Plot.Legend(location:Alignment.UpperLeft);
                    WpfPlot1.Refresh();
                });
            });

            // create a separate timer to update the GUI
            _renderTimer.Tick += (_, args) =>
            {
                try
                {
                    WpfPlot1.Refresh();
                }
                catch
                {
                    // ignore
                }
            };
            _renderTimer.Start();

            // Closed += (sender, args) =>
            // {
            //     _renderTimer?.Stop();
            // };
        }
    }
}
