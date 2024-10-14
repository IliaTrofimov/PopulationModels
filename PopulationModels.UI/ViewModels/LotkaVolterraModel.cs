using PopulationModels.UI.Computing;

namespace PopulationModels.UI.ViewModels;

public sealed class LotkaVolterraModel : ViewModelBase, IOdeModel
{
    private readonly RangeModelParameter alpha = new("alpha", 1, -5, 5);
    private readonly RangeModelParameter beta = new("beta", 1, -5, 5);
    private readonly RangeModelParameter gamma = new("gamma", 1, -5, 5);
    private readonly RangeModelParameter delta = new("delta", 1, -5, 5);

    private readonly RangeModelParameter[] parameters;
    private readonly RangeModelParameter[] initialValues;
    private readonly string[] variables = ["x - жертвы", "y - хищники"];

    public IEnumerable<RangeModelParameter> Parameters => parameters;
    public IEnumerable<RangeModelParameter> InitialValues => initialValues;
    public string Name => "Лотки-Вольтерра";
    public string Formula => "x' = (alpha - beta*y)x\ny' = (-gamma + delta*x)y";
    public IOdeSystem OdeSystem { get; }
    public string GetVariableName(int index) => variables[index];


    public LotkaVolterraModel()
    {
        parameters = [alpha, beta, gamma, delta];
        SetInternalPropertyChangedHandlers(alpha, beta, gamma, delta);
        
        var lotka = new LotkaVolterraSystem(alpha, beta, gamma, delta);
        OdeSystem = lotka;
        
        alpha.PropertyChanged += (sender, args) => lotka.Alpha = alpha; 
        beta.PropertyChanged += (sender, args) => lotka.Beta = beta; 
        gamma.PropertyChanged += (sender, args) => lotka.Gamma = gamma; 
        delta.PropertyChanged += (sender, args) => lotka.Delta = delta; 
        
        initialValues =
        [
            new RangeModelParameter("x0", 5, 0, 50),
            new RangeModelParameter("y0", 5, 0, 50)
        ];
        SetInternalPropertyChangedHandlers(initialValues);
    }
}