using MathNet.Numerics.LinearAlgebra;
using PopulationModels.UI.Models;

namespace PopulationModels.UI.Computing
{
    internal class OdeTraceCalculator : OdeTraceCalculatorBase
    {
        protected Vector<double>? initialPoint;

        public void SetPoint(double x, double y)
        {
            if (initialPoint == null)
                initialPoint = CreateVector.DenseOfArray([x, y]);
            else
            {
                initialPoint[0] = x;
                initialPoint[1] = y;
            }
        }

        public OdeTrace Solve(double maxT, double tStep, IOdeModel ode)
        {
            if (initialPoint == null)
                return new OdeTrace();
            int n = (int)(maxT / tStep);
            return SolveForPoint(initialPoint, maxT, n, ode);
        }
    }

}
