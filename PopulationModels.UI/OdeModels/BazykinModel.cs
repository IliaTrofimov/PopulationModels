using System.ComponentModel;
using PopulationModels.Computing.OdeSystems;
using PopulationModels.UI.Resources;
using PopulationModels.UI.ViewModels;
using PopulationModels.UI.ViewModels.ModelParameter;


namespace PopulationModels.UI.OdeModels;


public sealed class BazykinModel : ViewModelBase, IOdeModel
{
    private readonly RangeModelParameter a = new BazykinParameterA();
    private readonly RangeModelParameter b = new BazykinParameterB();
    private readonly RangeModelParameter e = new BazykinParameterE();
    private readonly RangeModelParameter c = new BazykinParameterC();
    private readonly RangeModelParameter d = new BazykinParameterD();
    private readonly RangeModelParameter m = new BazykinParameterM();
    private readonly RangeModelParameter p = new BazykinParameterP();
    
    private readonly BazykinSystem bazykinSystem;
    
    public IOdeSystem OdeSystem => bazykinSystem.Set(a, b, c, d, e, m, p);
    public IReadOnlyList<RangeModelParameter> Parameters { get; }
    public IReadOnlyList<RangeModelParameter> InitialValues { get; }
    public string Name => Localization.BazykinModel;
    public string Formula => "x' = Ax - Bxy / (1 + px) - Ex^2\n" +
                             "y' = -Cy + Dxy / (1 + px) - My^2";
    public string Description => Localization.BazykinModel_description;
    public IEnumerable<string> Examples => [];
    
    public string GetVariableName(int index) => index switch
    {
        0 => "x(t): " + Localization.Prey,
        1 => "y(t): " + Localization.Predators,
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };


    public BazykinModel()
    {
        bazykinSystem = new BazykinSystem(a, b, c, d, e, m, p);
        Parameters = [a, b, c, d, e, m, p];
        InitialValues =
        [
            new BazykinParameterX0(),
            new BazykinParameterY0()
        ];
        
        a.PropertyChanged += ParameterChangedHandler;
        b.PropertyChanged += ParameterChangedHandler;
        c.PropertyChanged += ParameterChangedHandler;
        d.PropertyChanged += ParameterChangedHandler;
        e.PropertyChanged += ParameterChangedHandler;
        m.PropertyChanged += ParameterChangedHandler;
        p.PropertyChanged += ParameterChangedHandler;
        InitialValues[0].PropertyChanged += ParameterChangedHandler;
        InitialValues[1].PropertyChanged += ParameterChangedHandler;
    }

    private void ParameterChangedHandler(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs is not ModelParameterChangedEventArgs parameterChangedArgs) return;
        
        var description = Math.Abs(parameterChangedArgs.NewValue) <= 5*1e-3
            ? GetParameterZeroedDescription(parameterChangedArgs.ParameterName)
            : parameterChangedArgs.NewValue - parameterChangedArgs.OldValue < 0
                ? GetParameterDecreasedDescription(parameterChangedArgs.ParameterName)
                : GetParameterIncreasedDescription(parameterChangedArgs.ParameterName);
        
        OnPropertyChanged(new ModelParameterChangedEventArgs(parameterChangedArgs, description));
    }

    internal static string GetParameterIncreasedDescription(string? parameter) =>
        parameter switch
        {
            "A" => Localization.Bazykin_A_increase,
            "B" => Localization.Bazykin_B_increase,
            "C" => Localization.Bazykin_C_increase,
            "D" => Localization.Bazykin_D_increase,
            "E" => Localization.Bazykin_E_increase,
            "M" => Localization.Bazykin_E_decrease,
            _   => ""
        };

    internal static string GetParameterDecreasedDescription(string? parameter) =>
        parameter switch
        {
            "A" => Localization.Bazykin_A_decrease,
            "B" => Localization.Bazykin_B_decrease,
            "C" => Localization.Bazykin_C_decrease,
            "D" => Localization.Bazykin_D_decrease,
            "E" => Localization.Bazykin_E_decrease,
            "M" => Localization.Bazykin_E_decrease,
            _   => ""
        };

    internal static string GetParameterZeroedDescription(string? parameter) =>
        parameter switch
        {
            "A" => Localization.Bazykin_A_zero,
            "B" => Localization.Bazykin_B_zero,
            "C" => Localization.Bazykin_C_zero,
            "D" => Localization.Bazykin_D_zero,
            "E" => Localization.Bazykin_E_zero,
            "M" => Localization.Bazykin_E_zero,
            _   => ""
        };
}