using PopulationModels.Computing.Matrix;

namespace PopulationModels.Computing.OdeSystems;

public class LotkaVolterraSystem : IOdeSystem
{
    public double Alpha, Beta, Gamma, Delta;
    
    public LotkaVolterraSystem(double alpha = 1, double beta = 1, double gamma = 0, double delta = 1)
    {
        Alpha = alpha;
        Beta = beta;
        Gamma = gamma;
        Delta = delta;
    }

    public void Derivatives(double t, IReadOnlySlice y, IMatrixSlice result)
    {
        result[0] = (Alpha - Beta * y[1]) * y[0];
        result[1] = (-Gamma + Delta * y[0]) * y[1];
    }
}