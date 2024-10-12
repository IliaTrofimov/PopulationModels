using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PopulationModels.UI.Computing;

using ScottPlot;
using ScottPlot.Plottables;

namespace PopulationModels.UI.PlottigModels
{
    internal class OdeTracePlot
    {
        public Scatter? Trace { get; private set; }
        public Scatter? Start { get; private set; }
        public Scatter? End { get; private set; }


        public void Clear(Plot plot)
        {
            plot.Remove(Trace);
            plot.Remove(Start);
            plot.Remove(End);
        }

        public void Draw(Plot plot, OdeTrace odeSolution)
        {
            Clear(plot);


            Start = plot.Add.ScatterPoints(new Coordinates[] { new(odeSolution.X0, odeSolution.Y0) }, Colors.Green);
            Start.MarkerSize = 10f;
            Start.MarkerShape = MarkerShape.FilledSquare;

            Trace = plot.Add.Scatter(odeSolution.XValues, odeSolution.YValues, Colors.Green);
            Trace.MarkerShape = MarkerShape.Eks;
            Trace.LineWidth = 2f;

            End = plot.Add.ScatterPoints(new Coordinates[] { new(odeSolution.Xn, odeSolution.Yn) }, Colors.Green);
            End.MarkerSize = 9;
            End.MarkerShape = MarkerShape.FilledTriangleUp;
        }
    }
}
