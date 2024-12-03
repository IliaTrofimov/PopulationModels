using System.Linq;
using PopulationModels.Computing.Matrix;
using PopulationModels.Computing.Ode;
using PopulationModels.UI.OdeModels;
using PopulationModels.UI.ViewModels.ModelParameter;
using ScottPlot;
using ScottPlot.AxisRules;
using ScottPlot.Plottables;


namespace PopulationModels.UI.Plotting;


public static class Plotter
{
    public static void ClearPlots(this Plot plot)
    {
        plot.Remove<Scatter>();
        plot.Remove<Annotation>();
        plot.Remove<Text>();
        plot.Remove<VerticalLine>();
        plot.Remove<HorizontalLine>();
    }
    
    public static void InitPlots(this Plot plot)
    {
        plot.SetAxisRules();

        plot.Axes.Bottom.Label.FontSize = 11;
        plot.Axes.Bottom.Label.Italic = true;
        plot.Axes.Bottom.Label.Bold = false;
        plot.Axes.Bottom.Label.Padding = 1;

        plot.Axes.Left.Label.FontSize = 11;
        plot.Axes.Left.Label.Italic = true;
        plot.Axes.Left.Label.Bold = false;
        plot.Axes.Bottom.Label.Padding = 1;
    }

    public static void SetAxisRules(this Plot plot, double maxX = double.PositiveInfinity, double maxY = double.PositiveInfinity)
    {
        plot.Axes.Rules.Clear();

        if (double.IsFinite(maxX) && double.IsFinite(maxY))
        {
            plot.Axes.Rules.Add(new MaximumBoundary(plot.Axes.Bottom, plot.Axes.Left,
                new AxisLimits(-0.1, maxX * 1.05, -0.1, maxY * 1.05)
            ));
        }
        else
        {
            plot.Axes.Rules.Add(new LockedLeft(plot.Axes.Bottom, -0.1));
            plot.Axes.Rules.Add(new LockedBottom(plot.Axes.Left, -0.1));
        }
    }
    
    
    private static Slice _timeData = new();
    
    public static (double maxX, double maxY) SolutionsPlot(this Plot plot, ContinuousMatrix solution, OdeInitialState initialState, IOdeModel ode, string xLabel, string yLabel)
    {
        _timeData.Dispose();
        _timeData = new Slice(solution.Columns, initialState.Start, initialState.End, useArrayPool: true);
        
        for (var i = 0; i < solution.Rows; i++)
        {
            var line = plot.Add.ScatterLine(_timeData, solution[i], plot.Add.Palette.GetColor(i));
            line.MaxRenderIndex = solution.Columns - 1;
            line.Data.MaxRenderIndex = solution.Columns - 1;
            line.LegendText = ode.GetVariableName(i);
            line.LineWidth = 3;
        }
        
        plot.Axes.Bottom.Label.Text = xLabel;
        plot.Axes.Left.Label.Text = yLabel;

        var maxY = 0.0;
        for (var i = 0; i < solution.Columns; i += 5)
        {
            maxY = Math.Max(maxY, solution[0, i]);
            maxY = Math.Max(maxY, solution[1, i]);
        }
        
        return (_timeData[^1], maxY);
    }
    
    
    public static (double maxX, double maxY) PhasePlot(this Plot plot, ContinuousMatrix solution, OdeInitialState initialState, IOdeModel ode, string label)
    {
        plot.DrawPhasePlotCurve(solution, label);
        plot.DrawPhasePlotStepPoints(solution);
        var bounds = plot.DrawPhasePlotTimePoints(solution, initialState);

        plot.Axes.Bottom.Label.Text = ode.GetVariableName(0);
        plot.Axes.Left.Label.Text = ode.GetVariableName(1);

        return bounds;
    }
    
    private static void DrawPhasePlotCurve(this Plot plot, ContinuousMatrix solution, string label)
    {
        var line = plot.Add.ScatterLine(solution[0], solution[1], Colors.Red);
        line.MaxRenderIndex = solution.Columns - 1;
        line.Data.MaxRenderIndex = solution.Columns - 1;
        line.LegendText = label;
        line.LineWidth = 3;
        line.Smooth = true;
    }

    private static void DrawPhasePlotStepPoints(this Plot plot, ContinuousMatrix solution)
    {
        var markersX = new double[Math.Min(50, solution.Columns)];
        var markersY = new double[Math.Min(50, solution.Columns)];
        var step = solution.Columns / markersX.Length;
        
        for (var i = 0; i < markersX.Length; i++)
        {
            var j = step * i;
            markersX[i] = solution[0, j];
            markersY[i] = solution[1, j];
        }
        
        var scatter = plot.Add.ScatterPoints(markersX, markersY, Colors.Red);
        scatter.MarkerStyle.Size = 5;
        scatter.MarkerStyle.Shape = MarkerShape.FilledCircle;
    }
    
    private static (double maxX, double maxY) DrawPhasePlotTimePoints(this Plot plot, ContinuousMatrix solution, OdeInitialState initialState)
    {
        double maxX = double.MinValue, maxY = double.MinValue;
        double minX = double.MaxValue, minY = double.MaxValue;
        for (var i = 0; i < solution.Columns; i += 10)
        {
            var x = solution[0, i];
            var y = solution[1, i];
            maxX = Math.Max(maxX, x);
            minX = Math.Min(minX, x);
            maxY = Math.Max(maxY, y);
            minY = Math.Min(minY, y);
        }
        
        var spreadX = Math.Abs(maxX - minX);
        var spreadY = Math.Abs(maxY - minY);
        var maxDist = Math.Sqrt(spreadX * spreadX + spreadY * spreadY) / 20;
        
        var markersX = new List<double>(50) { solution[0, 0], solution[0, solution.Columns - 1] };
        var markersY = new List<double>(50) { solution[1, 0], solution[1, solution.Columns - 1] };
        
        var text = plot.Add.Text("t_0 = 0", markersX[0], markersY[0]);
        text.LabelStyle.FontSize = 10;
        text.OffsetX = 10;
        text = plot.Add.Text($"t_{initialState.Steps} = {initialState.End:F2}", markersX[1], markersY[1]);
        text.OffsetX = 10;
        text.LabelStyle.FontSize = 10;

        for (var i = 1; i < solution.Columns; i++)
        {
            var xi = solution[0, i];
            var yi = solution[1, i];
            
            var nearestDist = double.MaxValue;
            for (var j = markersX.Count - 1; j >= 0 && nearestDist >= maxDist; j--)
            {
                var dist = Math.Sqrt(Math.Pow(markersX[j] - xi, 2) + Math.Pow(markersY[j] - yi, 2));
                nearestDist = Math.Min(dist, nearestDist);
            }
            if (nearestDist < maxDist)
                continue;
            
            markersX.Add(xi);
            markersY.Add(yi);

            var t = i * initialState.Step;
            var tLabel = plot.Add.Text($"t_{i} = {t:F2}", xi, yi);
            tLabel.OffsetX = 10;
            tLabel.LabelStyle.FontSize = 10;
        }
        
        var scatter = plot.Add.ScatterPoints(markersX, markersY, Colors.White);
        scatter.MarkerStyle.Size = 8;
        scatter.MarkerStyle.OutlineWidth = 2;
        scatter.MarkerStyle.OutlineColor = Colors.Red;

        return (maxX, maxY);
    }
}