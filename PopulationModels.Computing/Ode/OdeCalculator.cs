using PopulationModels.Computing.Matrix;
using PopulationModels.Computing.OdeSystems;
using PopulationModels.Computing.Solvers;


namespace PopulationModels.Computing.Ode;

public static class OdeCalculator
{
    public static ContinuousMatrix Solve(IOdeSystem ode, OdeInitialState state, Solvers.Solvers solver, int maxPoints = 2000)
    { 
        var approx = solver switch
        {
            Solvers.Solvers.Rk2         => RungeKuttaOptimized3.SecondOrder(state, ode.Derivatives, maxPoints),
            Solvers.Solvers.Rk4         => RungeKuttaOptimized3.FourthOrder(state, ode.Derivatives, maxPoints),
            Solvers.Solvers.Rk2Implicit => RungeKuttaOptimized3.ImplicitSecondOrder(state, ode.Derivatives, maxPoints),
            _                           => throw new NotImplementedException($"Unknown solver {solver}")
        };
        return approx;
    }
}