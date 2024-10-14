using MathNet.Numerics.LinearAlgebra;
using PopulationModels.UI.Computing;
using PopulationModels.UI.ViewModels;

namespace PopulationModels.UI.ViewModels;


public sealed class BazykinModelA : ViewModelBase, IOdeModel
{
    private readonly RangeModelParameter A = new("A", 1, -5, 5);
    private readonly RangeModelParameter B = new("B", 1, -5, 5);
    private readonly RangeModelParameter E = new("E", 0.005, -0.01, 0.01);
    private readonly RangeModelParameter C = new("C", 1, -5, 5);
    private readonly RangeModelParameter D = new("D", 1, -5, 5);
    private readonly RangeModelParameter M = new("M", 0.005, -0.01, 0.01);
    private readonly RangeModelParameter p = new("p", 0.1, 0, 1);

    private readonly RangeModelParameter[] parameters;
    private readonly RangeModelParameter[] initialValues;
    private readonly string[] variables = ["x - жертвы", "y - хищники"];

    public IEnumerable<RangeModelParameter> Parameters => parameters;
    public IEnumerable<RangeModelParameter> InitialValues => initialValues;
    public string Name => "Базыкина (1)";
    public string Formula => "x' = Ax - Bxy / (1 + px) - Ex^2\ny' = -Cy + Dxy / (1 + px) - My^2";
    public IOdeSystem OdeSystem { get; }
    
    public string GetVariableName(int index) => variables[index];

    
    public BazykinModelA()
    {
        parameters = [A, B, E, C, D, M, p];
        SetInternalPropertyChangedHandlers(A, B, E, C, D, M, p);

        var bazykin = new BazykinSystem(A, B, E, C, D, M, p);
        OdeSystem = bazykin;

        A.PropertyChanged += (sender, args) => bazykin.A = A; 
        B.PropertyChanged += (sender, args) => bazykin.B = B; 
        E.PropertyChanged += (sender, args) => bazykin.C = C; 
        C.PropertyChanged += (sender, args) => bazykin.E = E; 
        D.PropertyChanged += (sender, args) => bazykin.D = D; 
        M.PropertyChanged += (sender, args) => bazykin.M = M; 
        p.PropertyChanged += (sender, args) => bazykin.P = p; 

        initialValues =
        [
            new RangeModelParameter("x0", 5, 0, 50),
            new RangeModelParameter("y0", 5, 0, 50)
        ];
        SetInternalPropertyChangedHandlers(initialValues);
    }
}