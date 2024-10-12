using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using MathNet.Numerics.LinearAlgebra;

using PopulationModels.UI.Models;

namespace PopulationModels.UI.Computing
{
    internal class StreamPlotCalculator : OdeTraceCalculatorBase
    {
        private List<Vector<double>> initialPoints = null!;

        private const double updateThreshold = 1e-4;

        private double xMin, xMax, yMin, yMax;
        private readonly int steps;
        private bool shouldUpdate;

        public double XMin 
        {
            get => xMin;
            set => SetProperty(ref xMin, value);
        }
        public double XMax
        {
            get => xMax;
            set => SetProperty(ref xMax, value);
        }
        public double YMin
        {
            get => yMin;
            set => SetProperty(ref yMin, value);
        }
        public double YMax
        {
            get => yMax;
            set => SetProperty(ref yMax, value);
        }


        public StreamPlotCalculator(int steps)
        {
            this.steps = steps;
        }

        public StreamPlotCalculator(double xMin, double xMax, double yMin, double yMax, int steps) 
        {
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;
            this.steps = steps;
            InitPoints();
        }


        public Task<OdeTrace[]> SolveAsync(double maxT, double tStep, IOdeModel ode)
        {
            if (shouldUpdate) InitPoints();

            int n = (int)(maxT / tStep);
            var tasks = initialPoints.Select(y0 => Task.Run(() => SolveForPoint(y0, maxT, n, ode))).ToArray();
            return Task.WhenAll(tasks);
        }

        public OdeTrace[] Solve(double maxT, double tStep, IOdeModel ode)
        {
            if (shouldUpdate) InitPoints();
           
            int n = (int)(maxT / tStep);
            return initialPoints.Select(y0 => SolveForPoint(y0, maxT, n, ode)).ToArray();
        }


        private void InitPoints()
        {
            shouldUpdate = false;
            var xStep = Math.Abs(xMax - xMin) / steps;
            var yStep = Math.Abs(yMax - yMin) / steps;

            initialPoints = new List<Vector<double>>(steps * steps);

            for (int x = 0; x < steps; x++)
                for (int y = 0; y < steps; y++)
                    initialPoints.Add(CreateVector.DenseOfArray(new double[] { xMin + x * xStep, yMin + y * yStep }));
        }

        private void SetProperty(ref double prop, double value)
        {
            if (Math.Abs(prop - value) < updateThreshold) return;

            prop = value;
            shouldUpdate = true;
        }
    }

}
