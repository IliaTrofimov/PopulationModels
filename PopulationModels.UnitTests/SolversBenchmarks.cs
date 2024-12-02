using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.OdeSolvers;
using PopulationModels.Computing.Matrix;
using PopulationModels.Computing.Ode;
using PopulationModels.Computing.OdeSystems;
using PopulationModels.Computing.Solvers;
using Xunit.Abstractions;


namespace PopulationModels.UnitTests;

public class SolversBenchmarks(ITestOutputHelper Output)
{
    [Theory]
    [InlineData(100, 0.001)]
    [InlineData(1000, 0.001)]
    [InlineData(5000, 0.001)]
    [InlineData(15000, 0.001)]
    [InlineData(250000, 0.001)]
    [InlineData(100, 1e-6)]
    [InlineData(1000, 1e-6)]
    [InlineData(5000, 1e-6)]
    [InlineData(15000, 1e-6)]
    [InlineData(250000, 1e-6)]
    public async Task RunBenchmarks_default(int steps, double dt)
    {
        double t0 = 0, tn = dt * steps;
        double[] y0 = [1.0, 0.5];
        const int maxPoints = 2000;
        
        var initialState = new OdeInitialState(t0, tn, dt, y0);
        
        var rk2Opt = Task.Run(() => Benchmark("RK2 Optimized", 
            action: () => RungeKuttaOptimized3.SecondOrder(initialState, SystemFunc, maxPoints),
            after: FinalizeMatrix)
        );
        var rk4Opt = Task.Run(() => Benchmark("RK4 Optimized", 
            action: () => RungeKuttaOptimized3.FourthOrder(initialState, SystemFunc, maxPoints),
            after: FinalizeMatrix)
        );
        var rk4Opt_no_truncate = Task.Run(() => Benchmark("RK4 Optimized *", 
            action: () => RungeKuttaOptimized3.FourthOrder(initialState, SystemFunc, int.MaxValue),
            after: FinalizeMatrix)
        );
        var rk2ImpOpt = Task.Run(() => Benchmark("RK2 Implicit Optimized", 
            action: () => RungeKuttaOptimized3.ImplicitSecondOrder(initialState, SystemFunc, maxPoints),
            after: FinalizeMatrix)
        );
        var rk2 = Task.Run(() => Benchmark("RK2 Default", () =>
            {
                var x = RungeKutta.SecondOrder(CreateVector.Dense(y0), t0, tn, steps, SystemFunc);
                Assert.InRange(x.Length, steps - 1, steps + 1);
            })
        );
        var rk4 = Task.Run(() => Benchmark("RK4 Default", () =>
            {
                var x = RungeKutta.FourthOrder(CreateVector.Dense(y0), t0, tn, steps, SystemFunc);
                Assert.InRange(x.Length, steps - 1, steps + 1);
            })
        );
        
        var results = await Task.WhenAll(rk2, rk4, rk2Opt, rk4Opt, rk2ImpOpt, rk4Opt_no_truncate);
        
        Output.WriteLine($"STEPS: {steps}; dT: {dt:E2}; t0: {t0}; tn: {tn}");
        Output.WriteLine($"INITIAL STATE: {initialState};");

        Output.WriteLine("{0,-22}|{1,9}|{2,9}|{3,9}", "name", "min (ms)", "avg (ms)", "max (ms)");
        
        foreach (var result in results.OrderBy(res => res.Avg))
        {
            Output.WriteLine("{0,-22}|{1,9:F3}|{2,9:F3}|{3,9:F3}", result.Name, result.Min, result.Avg, result.Max);
        }
    }
    
    
    [Theory]
    [InlineData(10, 1e-2)]
    [InlineData(15, 1e-2)]
    [InlineData(25, 1e-2)]
    [InlineData(50, 1e-2)]
    [InlineData(100, 1e-2)]
    [InlineData(10, 1e-3)]
    [InlineData(15, 1e-3)]
    [InlineData(25, 1e-3)]
    [InlineData(50, 1e-3)]
    [InlineData(100, 1e-3)]
    [InlineData(10, 1e-4)]
    [InlineData(15, 1e-4)]
    [InlineData(25, 1e-4)]
    [InlineData(50, 1e-4)]
    [InlineData(100, 1e-4)]
    [InlineData(10, 1e-5)]
    [InlineData(15, 1e-5)]
    [InlineData(25, 1e-5)]
    [InlineData(50, 1e-5)]
    [InlineData(100, 1e-5)]
    public async Task RunBenchmarks_bazykin(int tMax, int dtOrder)
    {
        double[] y0 = [110, 50, 500];
        
        var initialState = new OdeInitialState(0.0, tMax, Math.Pow(10, dtOrder), y0);
        var bazykinOde = new BazykinSystem(1, 1, 1, 1,1, 1, 1);
        
        var rk2Opt = Task.Run(() => Benchmark("RK2 Optimized", 
            action: () => RungeKuttaOptimized3.SecondOrder(initialState, bazykinOde.Derivatives),
            after: FinalizeMatrix)
        );
        var rk4Opt = Task.Run(() => Benchmark("RK4 Optimized", 
            action: () => RungeKuttaOptimized3.FourthOrder(initialState, bazykinOde.Derivatives),
            after: FinalizeMatrix)
        );
        var rk2ImpOpt = Task.Run(() => Benchmark("RK2 Implicit Optimized", 
            action: () => RungeKuttaOptimized3.ImplicitSecondOrder(initialState, bazykinOde.Derivatives),
            after: FinalizeMatrix)
        );
       
        
        var results = await Task.WhenAll(rk2Opt, rk4Opt, rk2ImpOpt);
        
        Output.WriteLine($"INITIAL STATE: {initialState};");

        Output.WriteLine("{0,-22}|{1,9}|{2,9}|{3,9}", "name", "min (ms)", "avg (ms)", "max (ms)");
        
        foreach (var result in results.OrderBy(res => res.Avg))
        {
            Output.WriteLine("{0,-22}|{1,9:F3}|{2,9:F3}|{3,9:F3}", result.Name, result.Min, result.Avg, result.Max);
        }
    }
    
    
    //x' = sin(t) + y, y' = y - cos(t), x(0) = 1, y(0) = 0.5
    private static void SystemFunc(double t, IReadOnlySlice y, IMatrixSlice result)
    {
        result[0] = Math.Sin(t) + y[1];
        result[1] = y[1] - Math.Cos(t);
    }
    
    private static Vector<double> SystemFunc(double t, Vector<double> y)
    {
        return CreateVector.Dense([Math.Sin(t) + y[1], y[1] - Math.Cos(t)]);
    }

    private BenchmarkResult Benchmark<T>(string name, Func<T> action, int tries = 7, Action<T>? after = null)
    {
        TimeSpan min = TimeSpan.MaxValue, max = TimeSpan.Zero, sum = TimeSpan.Zero;

        for (var i = 0; i < tries; i++)
        {
            var sw = Stopwatch.StartNew();
            var res = action();
            sw.Stop();
            after?.Invoke(res);
            
            sum += sw.Elapsed;
            if (sw.Elapsed < min) min = sw.Elapsed;
            if (sw.Elapsed > max) max = sw.Elapsed;
        }
        
        return new BenchmarkResult(name, min.TotalMilliseconds, sum.TotalMilliseconds / tries, max.TotalMilliseconds);
    }
    
    private BenchmarkResult Benchmark(string name, Action action, int tries = 5)
    {
        TimeSpan min = TimeSpan.MaxValue, max = TimeSpan.Zero, sum = TimeSpan.Zero;

        for (var i = 0; i < tries; i++)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            
            sum += sw.Elapsed;
            if (sw.Elapsed < min) min = sw.Elapsed;
            if (sw.Elapsed > max) max = sw.Elapsed;
        }
        
        return new BenchmarkResult(name, min.TotalMilliseconds, sum.TotalMilliseconds / tries, max.TotalMilliseconds);
    }

    private static void FinalizeMatrix(ContinuousMatrix matrix)
    {
        matrix.Dispose();
    }
}