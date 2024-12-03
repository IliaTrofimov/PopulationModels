using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Interactivity;
using Avalonia.Threading;
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
    private OdeSolution currentSolution = null!;
    private bool hasSolutions;
    
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Reset()
    {
        InitializeComponent();
        PlotMain.Plot.InitPlots();
        RenderPlots(currentSolution, false);
    }
    
#region Event handlers
    
    private void MainWindow_Loaded(object? _, RoutedEventArgs e)
    {
        VM = DataContext as MainWindowViewModel ?? throw new NullReferenceException("DataContext must be of type MainWindowViewModel");
        Debug.WriteLine($"[EVENT] [{Environment.CurrentManagedThreadId}] MainWindow_Loaded");
        
        PlotMain.Plot.InitPlots();
        ExecuteSolutionSync();
        
        VM.PropertyChanged += ViewModelProperty_Changed;

        DispatcherTimer.Run(PlotterFunc, TimeSpan.FromMilliseconds(5), DispatcherPriority.MaxValue);
    }
    
    private void ViewModelProperty_Changed(object? sender, PropertyChangedEventArgs e)
    {
        var annotation = (e as ModelParameterChangedEventArgs)?.Description;
        Debug.WriteLine($"[ACTION] [{Environment.CurrentManagedThreadId}] Property {e.PropertyName} changed");

        ExecuteSolutionAsync(annotation);
    }
    
    private void ShowAboutWindow_Click(object? sender, RoutedEventArgs e)
    {
        if (!AboutWindow.IsCreated)
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

    private void ExecuteSolutionSync()
    {
        var initialValues = VM.OdeModel.Value.InitialValues.Select(x => x.Value).ToArray();
        var initialState = new OdeInitialState(0.0, VM.MaxTime, VM.TimeStep, initialValues);
        
        var sw = Stopwatch.StartNew();
        var solution = OdeCalculator.Solve(VM.OdeModel.Value.OdeSystem, initialState, VM.OdeAlgorithm.Value, MAX_PLOT_POINTS);
        
        #if DEBUG
        Debug.WriteLine($"[ACTION] [{Environment.CurrentManagedThreadId}] Solved in {sw.ElapsedMilliseconds:F3} ms");
        #endif
        
        currentSolution = new OdeSolution { InitialState = initialState, SolutionMatrix = solution };
        RenderPlots(currentSolution, false);
    }
    
    private bool PlotterFunc()
    {
        if (!hasSolutions) return true;
        
        hasSolutions = false;
        var lastSolution = currentSolution;

        if (!Monitor.TryEnter(PlotMain.Plot.Sync, 30)) return true;

        RenderPlots(lastSolution, true);
        
        Monitor.Exit(PlotMain.Plot.Sync);
        return true;
    }

    
    private Task ExecuteSolutionAsync(string? annotation = null)
    {
        return Task.Run(() =>
        {
            var initialValues = VM.OdeModel.Value.InitialValues.Select(x => x.Value).ToArray();
            var initialState = new OdeInitialState(0.0, VM.MaxTime, VM.TimeStep, initialValues);

            var sw = Stopwatch.StartNew();
            var solution = OdeCalculator.Solve(VM.OdeModel.Value.OdeSystem, initialState, VM.OdeAlgorithm.Value, MAX_PLOT_POINTS);
            currentSolution = new OdeSolution { Description = annotation, InitialState = initialState, SolutionMatrix = solution};
            ;
            hasSolutions = true;
            
            #if DEBUG
            Debug.WriteLine($"[ACTION] [{Environment.CurrentManagedThreadId}] Solved in {sw.ElapsedMilliseconds:F3} ms. Solution is {{solution}}");
            #else 
            Debug.WriteLine($"[ACTION] Solved in {sw.ElapsedMilliseconds:F3} ms.");
            #endif
        });
    }

    private void RenderPlots(OdeSolution solution, bool clean)
    {
        if (clean) PlotMain.Plot.ClearPlots();

        if (!string.IsNullOrEmpty(solution.Description))
        {
            var annotation = PlotMain.Plot.Add.Annotation(solution.Description, Alignment.UpperRight);
            annotation.LabelStyle.BackgroundColor = Colors.Aqua.Lighten(0.8);
            annotation.LabelStyle.SubpixelText = true;
        }
        
        var (maxX, maxY) = VM.DrawPhasePlot
            ? PlotMain.Plot.PhasePlot(solution.SolutionMatrix, solution.InitialState, VM.OdeModel.Value, "predators ~ prey")
            : PlotMain.Plot.SolutionsPlot(solution.SolutionMatrix, solution.InitialState, VM.OdeModel.Value, Localization.Time, Localization.Population);
        
        PlotMain.Plot.SetAxisRules(maxX, maxY);
        PlotMain.Plot.Axes.AutoScale();
        PlotMain.Refresh();
    }
    
#endregion
    
}