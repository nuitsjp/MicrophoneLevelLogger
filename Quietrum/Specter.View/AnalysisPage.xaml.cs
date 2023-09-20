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
                var signal = ViewModel
                    .ReadInputLevels(
                        analysisDevice.AudioRecord,
                        analysisDevice.DeviceRecord)
                    .Select(x => x.AsPrimitive())
                    .ToArray();
                maximumLength = Math.Max(maximumLength, signal.Length);
                WpfPlot1.Plot.AddSignal(
                    signal, 
                    label:$"{analysisDevice.StartTime:yyyy/MM/dd HH:mm}：{analysisDevice.Device} {analysisDevice.Direction}");
            }
                    
            WpfPlot1.Plot.AxisAutoX(margin: 0);

            // 0になてしまうとWpfPlot1がエラーとなるためその場合、仮に100を設定しておく
            maximumLength =
                maximumLength == 0
                    ? 100
                    : maximumLength;
            
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
}