using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.OdeSolvers;

namespace PopulationModels.UI.Computing;


public enum OdeAlgorithm
{
    RungeKutta2, RungeKutta4
}

public static class OdeCalculator
{
    public static double[][] Solve(IOdeSystem ode, double[] initialValues, double maxT, double dt, OdeAlgorithm algorithm = OdeAlgorithm.RungeKutta2)
    {
        var n = (int)(maxT / dt);
        var y0 = CreateVector.DenseOfArray(initialValues);
       
        var vectorSolution = algorithm switch
        {
            OdeAlgorithm.RungeKutta2 => RungeKutta.SecondOrder(y0, 0, maxT, n, ode.Derivatives),
            OdeAlgorithm.RungeKutta4 => RungeKutta.FourthOrder(y0, 0, maxT, n, ode.Derivatives),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm))
        };

        var solution = new double[vectorSolution[0].Count][];
        Parallel.For(0, vectorSolution[0].Count, i =>
        {
            solution[i] = new double[n];
            for (int j = 0; j < n; j++)
                solution[i][j] = vectorSolution[j][i];
        });
        return solution;
    }
}