using MathNet.Numerics.LinearAlgebra;


namespace PopulationModels.UI.Computing;

public class LotkaVolterraSystem(double alpha = 1, double beta = 1, double gamma = 0, double delta = 1) 
    : IOdeSystem
{
    public double Alpha = alpha, Beta = beta, Gamma = gamma, Delta = delta;
    
    public Vector<double> Derivatives(double t, Vector<double> y)
    {
        return CreateVector.DenseOfArray([
            (Alpha - Beta * y[1]) * y[0],
            (-Gamma + Delta * y[0]) * y[1]
        ]);
    }
}