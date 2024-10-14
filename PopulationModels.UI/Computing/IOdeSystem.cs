using MathNet.Numerics.LinearAlgebra;


namespace PopulationModels.UI.Computing;

public interface IOdeSystem
{
    public Vector<double> Derivatives(double t, Vector<double> y);
}