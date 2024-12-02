using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using PopulationModels.Computing.Matrix;
using PopulationModels.Computing.Ode;
using PopulationModels.UI.OdeModels;
using PopulationModels.UI.Plotting;
using PopulationModels.UI.Resources;
using PopulationModels.UI.ViewModels;
using ScottPlot;


namespace PopulationModels.UI.Views;

public partial class MainWindow : Window
{
    private const int MAX_PLOT_POINTS = 5000;
    
    private MainWindowViewModel VM = null!;
    private ContinuousMatrix solution;
    private OdeInitialState odeInitialState;
    private readonly TimeSpan renderLockTimeout = TimeSpan.FromMilliseconds(15);
    
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Reset()
    {
        InitializeComponent();
        PlotMain.Plot.InitPlots();
        RenderPlots(odeInitialState, solution, false);
    }

    
#region Event handlers
    
    private void MainWindow_Loaded(object? _, RoutedEventArgs e)
    {
        VM = DataContext as MainWindowViewModel ?? throw new NullReferenceException("DataContext must be of type MainWindowViewModel");
        Debug.WriteLine("[EVENT] MainWindow_Loaded");
        
        PlotMain.Plot.InitPlots();
        ExecuteSolution(false, true).GetAwaiter().GetResult();
        
        VM.PropertyChanged += ViewModelProperty_Changed;
    }
    
    private void ViewModelProperty_Changed(object? sender, PropertyChangedEventArgs e)
    {
        var annotation = (e as ModelParameterChangedEventArgs)?.Description;
        ExecuteSolution(true, annotation: annotation);
    }
    
    private void ShowAboutWindow_Click(object? sender, RoutedEventArgs e)
    {
        if (!AboutWindow.IsCreated){}
            new AboutWindow().ShowDialog(this);
    }
    
    internal void SetRussianLang_Click(object? sender, EventArgs e)
    {
        LocalizationHelper.ChangeLocalization("ru-RU", Reset);
    }
    
    internal void SetEnglishLang_Click(object? sender, EventArgs e)
    {
        LocalizationHelper.ChangeLocalization("en-EN", Reset);
    }

    private void SwitchLanguage_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Control {Tag: string languageTag})
            return;
        LocalizationHelper.ChangeLocalization(languageTag, Reset);
    }

    private void Close_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
    
#endregion
    
#region Actions

    private async Task ExecuteSolution(bool clean = false, bool sync = false, string? annotation = null)
    {
        var initialValues = VM.OdeModel.Value.InitialValues.Select(x => x.Value).ToArray();
        odeInitialState = new OdeInitialState(0.0, VM.MaxTime, VM.TimeStep, initialValues);

        if (sync)
        {
            solution.Dispose();
        
            var sw = Stopwatch.StartNew();
            solution = OdeCalculator.Solve(VM.OdeModel.Value.OdeSystem, odeInitialState, VM.OdeAlgorithm.Value, MAX_PLOT_POINTS);
            sw.Stop();
            Debug.WriteLine($"[ACTION] Solved in {sw.ElapsedMilliseconds:F3} ms");
            RenderPlots(odeInitialState, solution, clean, annotation);
            return;
        }
        if (Monitor.TryEnter(PlotMain.Plot.Sync, renderLockTimeout))
        {        
            solution.Dispose();
        
            var sw = Stopwatch.StartNew();
            solution = OdeCalculator.Solve(VM.OdeModel.Value.OdeSystem, odeInitialState, VM.OdeAlgorithm.Value, MAX_PLOT_POINTS);
            sw.Stop();
            Debug.WriteLine($"[ACTION] Solved in {sw.ElapsedMilliseconds:F3} ms");
            
            await Dispatcher.UIThread.InvokeAsync(() => RenderPlots(odeInitialState, solution, clean, annotation));
            Monitor.Exit(PlotMain.Plot.Sync);
        }
        else
        {
            Debug.WriteLine($"[ACTION] Render lock {PlotMain.Plot.Sync.GetHashCode():x} timeout");
        }
    }

    private void RenderPlots(OdeInitialState state, ContinuousMatrix currentSolution, bool clean,
                             string? description = null)
    {
        #if DEBUG
        Debug.WriteLine($"[ACTION] RenderPlots: solution is {currentSolution}");
        #endif

        if (clean) 
            PlotMain.Plot.ClearPlots();

        if (!string.IsNullOrEmpty(description))
        {
            var annotation = PlotMain.Plot.Add.Annotation(description, Alignment.UpperRight);
            annotation.LabelStyle.BackgroundColor = Colors.Aqua.Lighten(0.8);
            annotation.LabelStyle.SubpixelText = true;
        }
        
        var (maxX, maxY) = VM.DrawPhasePlot
            ? PlotMain.Plot.PhasePlot(currentSolution, state, VM.OdeModel.Value, "predators ~ prey")
            : PlotMain.Plot.SolutionsPlot(currentSolution, state, VM.OdeModel.Value, Localization.Time, Localization.Population);
        
        PlotMain.Plot.SetAxisRules(maxX, maxY);
        PlotMain.Plot.Axes.AutoScale();
        PlotMain.Refresh();
    }
    
#endregion
    
}