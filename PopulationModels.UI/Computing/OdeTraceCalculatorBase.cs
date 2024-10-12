using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.OdeSolvers;

using PopulationModels.UI.Models;

namespace PopulationModels.UI.Computing
{
    internal abstract class OdeTraceCalculatorBase
    {
        protected static OdeTrace SolveForPoint(Vector<double> y0, double maxT, int n, IOdeModel ode)
        {
            var traceVector = RungeKutta.SecondOrder(y0, 0, maxT, n, ode.Derivatives);
            var odeTrace = new OdeTrace(n);
            for (int i = 0; i < n; i++)
                odeTrace.SetValue(i, traceVector[i][0], traceVector[i][1]);
            return odeTrace;
        }
    }

}
