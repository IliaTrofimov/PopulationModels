using PopulationModels.Computing.Matrix;
using PopulationModels.Computing.Ode;


namespace PopulationModels.Computing.Solvers;

public static class RungeKuttaOptimized3
{
    public static ContinuousMatrix SecondOrder(
        OdeInitialState initialState,
        Action<double, IReadOnlySlice, IMatrixSlice> f,
        int resultPoints = 2000,
        bool? useArrayPool = null)
    {
        var resultMatrix = CreateResultMatrix(initialState, resultPoints, out var indexStep, useArrayPool);
        resultMatrix.SetColumn(0, initialState.Y0);
        var curr = resultMatrix.GetColumn(0);
        var prev = resultMatrix.GetColumn(0);
        
        using var k1 = new Slice(initialState.Variables, useArrayPool);
        using var k2 = new Slice(initialState.Variables, useArrayPool);
        using var temp = new Slice(initialState.Variables, useArrayPool);
        
        var t = initialState.Start;
        var dt = initialState.Step;
        var dtHalf = initialState.Step * 0.5;

        if (indexStep == 1)
        {
            for (var index = 1; index < resultMatrix.Columns; index++)
            {
                curr.Next();
                curr.Set(prev); // curr := copy(y[t])
            
                f(t, prev, k1); // y1 := f(t, y[t])
                
                f(t,  temp.EulerStep(dt, prev, k1), k2);  // y2 := y[t] + dt*f(t+1, dt*f(t, y[t]) )
                curr.RK2Combine(dtHalf, k1, k2);
            
                prev.Next();
                t += dt;
            }
        }
        else
        {
            using var tempCurr = new Slice(initialState.Variables, useArrayPool);
            tempCurr.Set(curr);
            
            for (var index = 1; index < resultMatrix.Columns; index++)
            {
                curr.Next();
                for (var innerIndex = 0; innerIndex < indexStep; innerIndex++)
                {
                    f(t, curr, k1); // y1 := f(t, y[t])
                    temp.EulerStep(dt, tempCurr, k1);
            
                    f(t, temp, k2);  // y2 := y[t] + dt*f(t+1, dt*f(t, y[t]) )
                    tempCurr.RK2Combine(dtHalf, k1, k2);

                    t += dt;
                }
                curr.Set(tempCurr);
                prev.Next();
            }
        }
        
        return resultMatrix;
    }
    
    public static ContinuousMatrix FourthOrder(
        OdeInitialState initialState,
        Action<double, IReadOnlySlice, MatrixSlice> f,
        int resultPoints = 2000,
        bool? useArrayPool = null)
    {
        var resultMatrix = CreateResultMatrix(initialState, resultPoints, out var indexStep, useArrayPool);
        resultMatrix.SetColumn(0, initialState.Y0);
        var curr = resultMatrix.GetColumn(0);
        var prev = resultMatrix.GetColumn(0);
        
        using var k1 = new Slice(initialState.Variables, useArrayPool);
        using var k2 = new Slice(initialState.Variables, useArrayPool);
        using var k3 = new Slice(initialState.Variables, useArrayPool);
        using var k4 = new Slice(initialState.Variables, useArrayPool);
        using var temp = new Slice(initialState.Variables, useArrayPool);
        
        var t = initialState.Start;
        var dt = initialState.Step;
        var dtHalf = initialState.Step * 0.5;
        var dtSixth = initialState.Step / 6.0;

        if (indexStep == 1)
        {
            for (var index = 1; index < resultMatrix.Columns; index++)
            {
                curr.Next();
                curr.Set(prev);
                var halfT = t + dtHalf;

                f(t, prev, k1);                   // k1 := f(t, y(t))
                temp.EulerStep(dtHalf, prev, k1); // temp := y(t) + dt/2*k1
                f(halfT, temp, k2);               // k2 := f(t + dt/2, temp) 
                temp.EulerStep(dtHalf, prev, k2); // temp := y(t) + dt/2*k2
                f(halfT, temp, k3);               // k3 := f(t + dt/2, temp)
                temp.EulerStep(dt, prev, k3);     // temp := y(t) + dt*k3
                f(t += dt, temp, k4);              // k4 := f(t + dt, temp)
            
                curr.RK4Combine(dtSixth, k1, k2, k3, k4);
            
                prev.Next();
            }
        }
        else
        {
            using var tempCurr = new Slice(initialState.Variables, useArrayPool);
            tempCurr.Set(curr);
            for (var index = 1; index < resultMatrix.Columns; index++)
            {
                curr.Next();
                for (var innerIndex = 0; innerIndex < indexStep; innerIndex++)
                {
                    var halfT = t + dtHalf;
                    
                    f(t, tempCurr, k1);                   // k1 := f(t, y(t))
                    temp.EulerStep(dtHalf, tempCurr, k1); // temp := y(t) + dt/2*k1
                    f(halfT, temp, k2);                   // k2 := f(t + dt/2, temp) 
                    temp.EulerStep(dtHalf, tempCurr, k2); // temp := y(t) + dt/2*k2
                    f(halfT, temp, k3);                   // k3 := f(t + dt/2, temp)
                    temp.EulerStep(dt, tempCurr, k3);     // temp := y(t) + dt*k3
                    f(t += dt, temp, k4);                  // k4 := f(t + dt, temp)
            
                    tempCurr.RK4Combine(dtSixth, k1, k2, k3, k4);
                }
                curr.Set(tempCurr);
                prev.Next();     
            }
        }
        
        return resultMatrix;
    }
    
    public static ContinuousMatrix ImplicitSecondOrder(
        OdeInitialState initialState,
        Action<double, IReadOnlySlice, MatrixSlice> f,
        int resultPoints = 2000,
        bool? useArrayPool = null)
    {
        var resultMatrix = CreateResultMatrix(initialState, resultPoints, out var indexStep, useArrayPool);
        resultMatrix.SetColumn(0, initialState.Y0);
        var curr = resultMatrix.GetColumn(0);
        var prev = resultMatrix.GetColumn(0);

        using var y1 = new Slice(initialState.Variables, useArrayPool: true);
        using var y2 = new Slice(initialState.Variables, useArrayPool: true);
        
        var t = initialState.Start;
        var dt = initialState.Step;
        var dtHalf = initialState.Step * 0.5;

        if (indexStep == 1)
        {
            for (var index = 1; index < resultMatrix.Columns; index++)
            {
                curr.Next();
                curr.Set(prev);
                
                f(t, prev, curr);                   // curr := f(t, y(t))
                y1.EulerStep(dt, prev, curr);  // y1 := y(t) + dt*f(t, y(t))
                f(t, y1, y2);                       // y2 := f(t+dt, y1)
                curr.RK1ImplicitCombine(dtHalf, prev, y2);
            
                prev.Next();
                t += dt;
            }   
        }
        else
        {
            using var tempCurr = new Slice(initialState.Variables, useArrayPool);
            tempCurr.Set(curr);
            
            for (var index = 1; index < resultMatrix.Columns; index++)
            {
                curr.Next();
                for (var innerIndex = 0; innerIndex < indexStep; innerIndex++)
                {
                    f(t, tempCurr, y1);             // y1 := f(t, y(t))
                    y2.EulerStep(dt, prev, y1); // y1 := y(t) + dt*f(t, y(t))
                    f(t, y1, y2);                   // y2 := f(t+dt, y1)
                    tempCurr.RK1ImplicitCombine(dtHalf, y1, y2);
                    
                    t += dt;
                }
                curr.Set(tempCurr);
                prev.Next();
            }
        }
        
        return resultMatrix;
    }
    
    
    private static ContinuousMatrix CreateResultMatrix(OdeInitialState initialState, int maxPointsCount, out int step, bool? useArrayPool)
    {
        if (initialState.Steps <= maxPointsCount)
        {
            step = 1;
            return new ContinuousMatrix(initialState.Variables, initialState.Steps, useArrayPool);
        }
        step = initialState.Steps / maxPointsCount;
        return new ContinuousMatrix(initialState.Variables, maxPointsCount, useArrayPool);
    }
}