using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PopulationModels.UI.Computing;
using PopulationModels.UI.ViewModels;
using ScottPlot;
using ScottPlot.AxisRules;
using ScottPlot.Plottables;


namespace PopulationModels.UI.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? viewModel;
    private MainWindowViewModel ViewModel => viewModel ??= (MainWindowViewModel)DataContext;
    


    public MainWindow()
    {
        InitializeComponent();
        Debug.WriteLine("MainWindow: .ctor");
    }
    
    private void MainWindow_Loaded(object? _, RoutedEventArgs e)
    {
        Debug.WriteLine("MainWindow_Loaded");
        InitPlots();
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Debug.WriteLine($"ViewModel_PropertyChanged: sender: {sender?.GetType()}");
        RedrawPlots();
    }
    
    private void ForceUpdate_Click(object? sender, RoutedEventArgs e)
    {
        Debug.WriteLine("ForceUpdate_Click");
        RedrawPlots();
    }
    
    private void InitPlots()
    {
        PlotMain.Plot.Add.HorizontalLine(0, color: Colors.Black);
        PlotMain.Plot.Axes.Rules.Add(new LockedLeft(PlotMain.Plot.Axes.Bottom, 0));
        //PlotMain.Plot.Axes.SetLimitsX(0, double.PositiveInfinity, PlotMain.Plot.Axes.Bottom);
    }

    private void ClearPlots()
    {
        PlotMain.Plot.Remove<Scatter>();
    }

    private void RedrawPlots()
    {
        Debug.WriteLine("RedrawPlots: start");
        
        ClearPlots();
        PlotMain.Plot.Axes.SetLimitsX(0, ViewModel.MaxTime);
        
        var sw = Stopwatch.StartNew();
        
        var initialValues = ViewModel.SelectedOdeModel.InitialValues.Select(x => x.Value).ToArray();
        var solution = OdeCalculator.Solve(ViewModel.SelectedOdeModel.OdeSystem, initialValues, ViewModel.MaxTime, ViewModel.TimeStep);
        var time = Enumerable.Range(0, solution[0].Length).Select(t => t * ViewModel.TimeStep).ToArray();
        
        Debug.WriteLine($"RedrawPlots: ODE system is solved in {sw.ElapsedMilliseconds:F2} ms");
        
        for (var i = 0; i < solution.Length; i++)
        {
            var plotX = PlotMain.Plot.Add.ScatterLine(time, solution[i]);
            plotX.LegendText = ViewModel.SelectedOdeModel.GetVariableName(i) + "(t)";
        }
        
        Debug.WriteLine("RedrawPlots: plots are done"); 
        PlotMain.Refresh();
    }
}