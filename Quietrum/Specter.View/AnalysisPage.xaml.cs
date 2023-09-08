using System;
using System.Windows;
using ScottPlot;
using Specter.ViewModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using Specter.ViewModel.AnalysisPage;

namespace Specter.View;

public partial class AnalysisPage : UserControl
{
    public AnalysisPage()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private AnalysisPageViewModel ViewModel => (AnalysisPageViewModel)DataContext;

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        ViewModel.AnalysisDevices.CollectionChanged += AnalysisDevicesOnCollectionChanged;
    }

    private void AnalysisDevicesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            WpfPlot1.Plot.Clear();

            int maximumLength = 0;
            foreach (var analysisDevice in ViewModel.AnalysisDevices)
            {
                var signal = analysisDevice.GetInputLevels().Select(x => x.AsPrimitive()).ToArray();
                maximumLength = Math.Max(maximumLength, signal.Length);
                WpfPlot1.Plot.AddSignal(
                    signal, 
                    label:$"{analysisDevice.StartTime:yyyy/MM/dd HH:mm}：{analysisDevice.Device} {analysisDevice.Direction}");
            }
                    
            WpfPlot1.Plot.AxisAutoX(margin: 0);

            // RecordingConfig config = ViewModel.RecordingConfig;
            // // データの全長から、デフォルトの表示幅分を引いた値をデフォルトのx軸の最小値とする
            // var xMin =
            //     // データの全長
            //     config.RecordingLength
            //     // 表示時間を表示間隔で割ることで、表示幅を計算する
            //     - (int)(DisplayWidth / ViewModel.RecordingConfig.RefreshRate.Interval);
            WpfPlot1.Plot.SetAxisLimits(
                xMin: 0, xMax: maximumLength,
                yMin: -90, yMax: 0);
            WpfPlot1.Plot.XAxis.SetBoundary(0, maximumLength);
            WpfPlot1.Plot.YAxis.SetBoundary(-90, 0);
            WpfPlot1.Plot.XAxis.TickLabelFormat(x => $"{(x / (RecordingConfig.Default.WaveFormat.SampleRate / 1000)):#0.0[s]}");
            WpfPlot1.Configuration.LockVerticalAxis = true;
            WpfPlot1.Plot.Legend(location:Alignment.UpperLeft);
            WpfPlot1.Refresh();
        });
    }

    private void AnalysisPage_OnGotFocus(object sender, RoutedEventArgs e)
    {
    }
}