using PopulationModels.Computing.Solvers;
using PopulationModels.UI.OdeModels;
using PopulationModels.UI.ViewModels.ModelParameter;


namespace PopulationModels.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public SelectableModelParameter<double> TimeStep { get; } = new("Δt", 1, [0.1, 0.01, 1e-3, 1e-4, 1e-5, 1e-6]);
    public SelectableModelParameter<double> MaxTime { get; } = new("max T", 4, [0.5, 1, 2, 5, 10, 15, 20, 25, 50, 100]);
    public EnumModelParameter<Solvers> OdeAlgorithm { get; } = new ("Algorithm", Solvers.Rk2);
    public SelectableModelParameter<IOdeModel> OdeModel { get; } = new ("OdeModel",
    [
        new BazykinModel2(),
        new BazykinModelSmall(),
    ]);

    private bool drawPhasePlot;
    public bool DrawPhasePlot
    {
        get => drawPhasePlot;
        set => SetProperty(ref drawPhasePlot, value);
    }
    
    
    public MainWindowViewModel()
    {
        foreach (var model in OdeModel.Values) 
            SetInternalPropertyChangedHandlers(model);
        SetInternalPropertyChangedHandlers(MaxTime, OdeAlgorithm, OdeModel, TimeStep);
    }
}