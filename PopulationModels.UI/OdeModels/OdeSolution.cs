using PopulationModels.Computing.Matrix;
using PopulationModels.Computing.Ode;


namespace PopulationModels.UI.OdeModels;

public class OdeSolution
{
    public OdeInitialState InitialState { get; set; }
    public ContinuousMatrix SolutionMatrix { get; set; }
    public string? Description { get; set; } = "";
}