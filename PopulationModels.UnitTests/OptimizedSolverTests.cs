using PopulationModels.Computing.Matrix;
using PopulationModels.Computing.Ode;
using PopulationModels.Computing.Solvers;
using Xunit.Abstractions;

namespace PopulationModels.UnitTests;

public class OptimizedSolverTests(ITestOutputHelper Output)
{
    private const int steps = 500;
    private const double dt = 0.001;
    private const double t0 = 0, tn = dt * steps;
    double[] y0 = [1.0, 0.5];
    
    
    [Fact]
    public void Test_RK2()
    {
        var initialState = new OdeInitialState(t0, tn, dt, y0);
        var actual = RungeKuttaOptimized3.SecondOrder(initialState, SystemFunc);
        var exact = CreateExactSolution(actual);

        Output.WriteLine($"APPROX actual: {actual}");

        Helpers.AssertEqual(exact, actual, "exact", dt*2);
    }
    
    [Fact]
    public void Test_RK2_truncated()
    {
        var initialState = new OdeInitialState(t0, tn, dt, y0);
        var actual = RungeKuttaOptimized3.SecondOrder(initialState, SystemFunc, resultPoints: steps / 5);
        var exact = CreateExactSolution(actual, dt * 5);

        Output.WriteLine($"APPROX actual: {actual}");

        Helpers.AssertEqual(exact, actual, "exact", dt*2);
    }

    [Fact]
    public void Test_RK4()
    {
        var initialState = new OdeInitialState(t0, tn, dt, y0);
        var actual = RungeKuttaOptimized3.FourthOrder(initialState, SystemFunc);
        var exact = CreateExactSolution(actual);
        
        Output.WriteLine($"APPROX actual: {actual}");
        
        Helpers.AssertEqual(exact, actual, "exact", dt*2);
    }
    
    [Fact]
    public void Test_RK4_truncated()
    {
        var initialState = new OdeInitialState(t0, tn, dt, y0);
        var actual = RungeKuttaOptimized3.FourthOrder(initialState, SystemFunc, resultPoints: steps / 5);
        var exact = CreateExactSolution(actual, dt * 5);

        Output.WriteLine($"APPROX actual: {actual}");

        Helpers.AssertEqual(exact, actual, "exact", dt*2);
    }
    
    [Fact]
    public void Test_RK2Implicit()
    {
        var initialState = new OdeInitialState(t0, tn, dt, y0);
        var actual = RungeKuttaOptimized3.ImplicitSecondOrder(initialState, SystemFunc);
        var exact = CreateExactSolution(actual);

        Output.WriteLine($"APPROX actual: {actual}");
        
        Helpers.AssertEqual(exact, actual, "exact", dt*2);
    }

    [Fact]
    public void Test_RK2Implicit_truncated()
    {
        var initialState = new OdeInitialState(t0, tn, dt, y0);
        var actual = RungeKuttaOptimized3.ImplicitSecondOrder(initialState, SystemFunc, resultPoints: steps / 5);
        var exact = CreateExactSolution(actual, dt * 5);

        Output.WriteLine($"APPROX actual: {actual}");

        Helpers.AssertEqual(exact, actual, "exact", dt*2);
    }
    
    
    //x' = sin(t) + y, y' = y - cos(t), x(0) = 1, y(0) = 0.5
    private static void SystemFunc(double t, IReadOnlySlice y, IMatrixSlice result)
    {
        result[0] = Math.Sin(t) + y[1];
        result[1] = y[1] - Math.Cos(t);
    }

    private static (double x, double y) SolutionFunc(double t)
    {
        return (0.5*(Math.Sin(t) - Math.Cos(t) + 3), 0.5*(Math.Cos(t) - Math.Sin(t)));
    }
    
    
    private ContinuousMatrix CreateExactSolution(ContinuousMatrix actual, double _dt = -1)
    {
        _dt = _dt <= 0 ? dt : _dt;
        var exact = new ContinuousMatrix(actual.Rows, actual.Columns);
        var time = new ContinuousMatrix(1, actual.Columns);
        for (var i = 0; i < actual.Columns; i++)
        {
            time[0, i] = t0 + i * _dt;
            (exact[0, i], exact[1, i]) = SolutionFunc(t0 + i * _dt);
        }

        Output.WriteLine($"TIME: {time}");
        Output.WriteLine($"EXACT: {exact}");
        return exact;
    }
}


public record BenchmarkResult(string Name, double Min, double Avg, double Max);