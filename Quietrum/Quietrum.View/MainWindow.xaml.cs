using Quietrum.ViewModel;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Quietrum.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _renderTimer = new()
        {
            Interval = TimeSpan.FromMilliseconds(10)
        };

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // plot the data array only once
            WpfPlot1.Plot.AddSignal(viewModel.LiveData);
            WpfPlot1.Plot.AxisAutoX(margin: 0);
            WpfPlot1.Plot.SetAxisLimits(
                xMin: viewModel.LiveData.Length / 2, xMax: viewModel.LiveData.Length,
                yMin: -1, yMax: 2.5);
            WpfPlot1.Plot.XAxis.SetBoundary(0, viewModel.LiveData.Length);
            WpfPlot1.Plot.YAxis.SetBoundary(-1, 2.5);
            WpfPlot1.Plot.XAxis.TickLabelFormat(x => $"{(((viewModel.LiveData.Length - x) * -1 * viewModel.Period.TotalMilliseconds) / 1000d):#0.0[s]}");
            WpfPlot1.Configuration.LockVerticalAxis = true;
            WpfPlot1.Refresh();

            // create a separate timer to update the GUI
            _renderTimer.Tick += (sender, args) =>
            {
                WpfPlot1.Refresh();
            };
            _renderTimer.Start();

            Closed += (sender, args) =>
            {
                _renderTimer?.Stop();
            };
        }
    }
}
