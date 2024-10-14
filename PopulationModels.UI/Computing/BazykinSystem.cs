using MathNet.Numerics.LinearAlgebra;


namespace PopulationModels.UI.Computing;

public class BazykinSystem(double a = 1, double b = 1, double e = 0, double c = 1, double d = 1, double m = 0, double p = 1) 
    : IOdeSystem
{
    public double A = a, B = b, E = e, C = c, D = d, M = m, P = p;
    
    public Vector<double> Derivatives(double t, Vector<double> y)
    {
        return CreateVector.DenseOfArray([
            A*y[0] - B*y[0]*y[1]/ (1 + P*y[0]) * E*Math.Pow(y[0], 2),
            -C*y[1] - D*y[0]*y[1]/ (1 + P*y[0]) * M*Math.Pow(y[1], 2)
        ]);
    }
}

public class Bazykin3System(double a = 1, double b = 1, double e = 0, double c = 1, double d = 1, double m = 0, double p = 1) 
    : IOdeSystem
{
    public double A = a, B = b, E = e, C = c, D = d, M = m, P = p;
    
    public Vector<double> Derivatives(double t, Vector<double> y)
    {
        return CreateVector.DenseOfArray([
            A*y[0] - B*y[0]*y[1]/ (1 + P*y[0]) * E*Math.Pow(y[0], 2),
            -C*y[1] - D*y[0]*y[1]/ (1 + P*y[0]) * M*Math.Pow(y[1], 2)
        ]);
    }
}