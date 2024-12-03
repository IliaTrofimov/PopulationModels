using System.ComponentModel;
using PopulationModels.Computing.OdeSystems;
using PopulationModels.UI.Resources;
using PopulationModels.UI.ViewModels;
using PopulationModels.UI.ViewModels.ModelParameter;


namespace PopulationModels.UI.OdeModels;

public sealed class BazykinModelWithTime : ViewModelBase, IOdeModel
{
    private readonly RangeModelParameter a = new BazykinParameterA();
    private readonly RangeModelParameter b = new BazykinParameterB();
    private readonly RangeModelParameter e = new BazykinParameterE();
    private readonly RangeModelParameter c = new BazykinParameterC();
    private readonly RangeModelParameter d = new BazykinParameterD();
    private readonly RangeModelParameter m = new BazykinParameterM();
    private readonly RangeModelParameter p = new BazykinParameterP();
    private readonly RangeModelParameter dayPeriod = new BazykinParameterDayPeriod();

    private readonly BazykinSystemWithTime bazykinSystem;
    
    public IOdeSystem OdeSystem => bazykinSystem.Set(a, b, c, d, e, m, p, dayPeriod);
    public IReadOnlyList<RangeModelParameter> Parameters { get; }
    public IReadOnlyList<RangeModelParameter> InitialValues { get; }
    public string Name => Localization.BazykinModel + " time";
    public string Formula => "x' = Ax - B(t)*xy / (1 + px) - Ex^2\n" +
                             "y' = -Cy + D(t)*xy / (1 + px) - My^2\n" +
                             "B(t) = B_0 * sin(pi / dayPeriod * t)\n" +
                             "D(t) = D_0 * sin(pi / dayPeriod * t)";
    public string Description => Localization.BazykinModel_description;
    public IEnumerable<string> Examples => [];
    
    public string GetVariableName(int index) => index switch
    {
        0 => "x(t): " + Localization.Prey,
        1 => "y(t): " + Localization.Predators,
        2 => "day(t)",
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };


    public BazykinModelWithTime()
    {
        bazykinSystem = new BazykinSystemWithTime(a, b, c, d, e, m, p, dayPeriod);
        Parameters = [a, b, c, d, e, m, p, dayPeriod];
        InitialValues =
        [
            new BazykinParameterX0(),
            new BazykinParameterY0(),
            new RangeModelParameter("day period", 0, 0, 0) { IsReadOnly = true },
        ];
        
        a.PropertyChanged += ParameterChangedHandler;
        b.PropertyChanged += ParameterChangedHandler;
        c.PropertyChanged += ParameterChangedHandler;
        d.PropertyChanged += ParameterChangedHandler;
        e.PropertyChanged += ParameterChangedHandler;
        m.PropertyChanged += ParameterChangedHandler;
        p.PropertyChanged += ParameterChangedHandler;
        dayPeriod.PropertyChanged += ParameterChangedHandler;
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