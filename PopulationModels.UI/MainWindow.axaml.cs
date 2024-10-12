using Avalonia.Controls;

using PopulationModels.UI.ViewModels;
using PopulationModels.UI.Computing;
using System.Threading.Tasks;
using ScottPlot;
using ScottPlot.AxisRules;
using ScottPlot.Plottables;
using Avalonia.Input;
using PopulationModels.UI.PlottigModels;
using System.Linq;


namespace PopulationModels.UI.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel? viewModel;
        private MainWindowViewModel ViewModel => viewModel ??= (MainWindowViewModel)DataContext;


        private readonly StreamPlotCalculator streamPlotCalculator;
        private const int streamPlotPoints = 10;

        private readonly OdeTraceCalculator odeTraceCalculator;
        private readonly OdeTracePlot odeTracePlot = new OdeTracePlot();


        public MainWindow()
        {
            InitializeComponent();

            odeTraceCalculator = new OdeTraceCalculator();
            streamPlotCalculator = new StreamPlotCalculator(streamPlotPoints)
            {
                XMin = 0,
                XMax = 10,
                YMin = 0,
                YMax = 10
            };
        }

        private void MainWindow_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            InitPlots();
            ResetPlots();
            UpdateStreamPlot(false);
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SetAxesRules();
            UpdateStreamPlot(true);
        }

        private void SetAxesRules()
        {
            Plot_Xt.Plot.Axes.Rules.Clear();
            Plot_Yt.Plot.Axes.Rules.Clear();

            Plot_Xt.Plot.Axes.Rules.Add(new LockedHorizontal(Plot_Xt.Plot.Axes.Bottom, 0, ViewModel.MaxTime));
            Plot_Yt.Plot.Axes.Rules.Add(new LockedHorizontal(Plot_Yt.Plot.Axes.Bottom, 0, ViewModel.MaxTime));

        }

        private void InitPlots()
        {
            Plot_Xt.Plot.Axes.AutoScaleExpandY(Plot_Xt.Plot.Axes.Left);
            Plot_Yt.Plot.Axes.AutoScaleExpandY(Plot_Yt.Plot.Axes.Left);

            Plot_Xt.Plot.Axes.Rules.Add(new LockedHorizontal(Plot_Xt.Plot.Axes.Bottom, 0, ViewModel.MaxTime));
            Plot_Yt.Plot.Axes.Rules.Add(new LockedHorizontal(Plot_Yt.Plot.Axes.Bottom, 0, ViewModel.MaxTime));
            Plot_XYt.Plot.Axes.Rules.Add(new MaximumBoundary(Plot_XYt.Plot.Axes.Bottom, Plot_XYt.Plot.Axes.Left, new AxisLimits(-0.5, 50, -0.5, 50)));
        }

        private void AddNewStreamPlotPoint(object sender, PointerPressedEventArgs args)
        {
            if (args.ClickCount != 2) return;

            var screenPoint = args.GetCurrentPoint(sender as Control).Position;
            var point = Plot_XYt.Plot.GetCoordinates(new Pixel(screenPoint.X, screenPoint.Y));

            odeTraceCalculator.SetPoint(point.X, point.Y);

            Plot_Xt.Plot.Clear();
            Plot_Yt.Plot.Clear();

            Plot_Xt.Plot.Add.HorizontalLine(0, color: Colors.Black);
            Plot_Yt.Plot.Add.HorizontalLine(0, color: Colors.Black);
            DrawMainTrace();
        }

        private void ResetPlots()
        {
            Plot_Xt.Plot.Clear();
            Plot_Yt.Plot.Clear();
            Plot_XYt.Plot.Clear();

            Plot_Xt.Plot.Add.HorizontalLine(0, color: Colors.Black);
            Plot_Yt.Plot.Add.HorizontalLine(0, color: Colors.Black);

            Plot_XYt.Plot.Add.HorizontalLine(0, color: Colors.Red);
            Plot_XYt.Plot.Add.VerticalLine(0, color: Colors.Blue);
        }

        private async Task UpdateStreamPlot(bool refresh)
        {
            var streamPlotTask = streamPlotCalculator.SolveAsync(5, 0.01, ViewModel.SelectedOdeModel).ConfigureAwait(true);
            if (refresh)
                InitPlots();

            Plot_XYt.Plot.Clear();
            DrawMainTrace();

            var streamPlotTraces = await streamPlotTask;
            foreach (var trace in streamPlotTraces)
            {
                var scat = Plot_XYt.Plot.Add.ScatterLine(trace.XValues, trace.YValues, Colors.Gray);
                scat.LineWidth = 0.5f;
            }
            Plot_XYt.Refresh();
        }

        private void DrawMainTrace()
        {
            var mainTrace = odeTraceCalculator.Solve(ViewModel.MaxTime, ViewModel.TimeStep, ViewModel.SelectedOdeModel);
            if (!mainTrace.HasValue)
                return;

            var time = Enumerable.Range(0, mainTrace.XValues.Length).Select(i => i * ViewModel.TimeStep).ToArray();

            Plot_Xt.Plot.Add.ScatterLine(time, mainTrace.XValues, Colors.Blue);
            Plot_Yt.Plot.Add.ScatterLine(time, mainTrace.YValues, Colors.Red);
            Plot_Yt.Refresh();
            Plot_Xt.Refresh();

            odeTracePlot.Draw(Plot_XYt.Plot, mainTrace);
            
        }
    }
}