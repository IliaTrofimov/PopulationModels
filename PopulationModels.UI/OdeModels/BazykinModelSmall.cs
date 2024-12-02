using System.ComponentModel;
using PopulationModels.Computing.OdeSystems;
using PopulationModels.UI.Resources;
using PopulationModels.UI.ViewModels;
using PopulationModels.UI.ViewModels.ModelParameter;


namespace PopulationModels.UI.OdeModels;

public sealed class BazykinModelSmall : ViewModelBase, IOdeModel
{
    private readonly RangeModelParameter a = new BazykinParameterA();
    private readonly RangeModelParameter e = new BazykinParameterE();
    private readonly RangeModelParameter c = new BazykinParameterC();
    private readonly RangeModelParameter m = new BazykinParameterM();

    private readonly BazykinSystem bazykinSystem;

    public IOdeSystem OdeSystem => bazykinSystem.Set(a, 1.0, c, 1.0, e, m, 1.0);
    public IReadOnlyList<RangeModelParameter> Parameters { get; }
    public IReadOnlyList<RangeModelParameter> InitialValues { get; }
    public string Name => Localization.BazykinModelSmall;
    public string Formula => "x' = Ax - xy / (1 + x) - Ex^2\n" +
                             "y' = -Cy + xy / (1 + x) - My^2";
    public string Description => Localization.BazykinModel_description;

    public IEnumerable<string> Examples => [];
    
    public string GetVariableName(int index) => index switch
    {
        0 => "x(t): " + Localization.Prey,
        1 => "y(t): " + Localization.Predators,
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };
    
    public BazykinModelSmall()
    {
        bazykinSystem = new BazykinSystem(a, 1.0, c, 1.0, e, m, 1.0);
        Parameters = [a, c, e, m];
        InitialValues =
        [
            new BazykinParameterX0(),
            new BazykinParameterY0()
        ];
        
        a.PropertyChanged += ParameterChangedHandler;
        c.PropertyChanged += ParameterChangedHandler;
        e.PropertyChanged += ParameterChangedHandler;
        m.PropertyChanged += ParameterChangedHandler;
    }
    
    private void ParameterChangedHandler(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs is not ModelParameterChangedEventArgs parameterChangedArgs) return;
        
        var description = Math.Abs(parameterChangedArgs.NewValue) <= 5*1e-3
            ? BazykinModel2.GetParameterZeroedDescription(parameterChangedArgs.ParameterName)
            : parameterChangedArgs.NewValue - parameterChangedArgs.OldValue < 0
                ? BazykinModel2.GetParameterDecreasedDescription(parameterChangedArgs.ParameterName)
                : BazykinModel2.GetParameterIncreasedDescription(parameterChangedArgs.ParameterName);
        
        OnPropertyChanged(new ModelParameterChangedEventArgs(parameterChangedArgs, description));
    }
}