namespace PopulationModels.Computing.Matrix;


/// <summary>
/// Helper methods for execution some vector operations used in ODE solvers.
/// </summary>
public static class VectorOp
{
    /// <summary>Execute: <c>this[i] := this[i] + value</c></summary>
    public static T Add<T>(this T @this, double value) where T : IMatrixSlice
    {
        for (var i = 0; i < @this.Length; i++)
            @this[i] += value;
        return @this;
    }

    
    /// <summary>Execute: <c>this[i] := slice[i]*multConst</c></summary>
    public static TLeft CMul<TLeft, TRight>(this TLeft @this, TRight other, double multConst)
        where TLeft : IMatrixSlice
        where TRight : IMatrixSlice
    {
        for (var i = 0; i < @this.Length; i++)
            @this[i] = other[i] * multConst;
        return @this;
    }
    
    /// <summary>Execute: <c>this[i] := slice[i]*multConst</c></summary>
    public static T CMul<T>(this T @this, double multConst) where T : IMatrixSlice
    {
        for (var i = 0; i < @this.Length; i++)
            @this[i] *= multConst;
        return @this;
    }
    
    /// <summary>Execute: <c>this[i] := this[i] + slice[i]*multConst</c></summary>
    public static TLeft AddCMul<TLeft, TRight>(this TLeft @this, TRight other, double multConst)
        where TLeft : IMatrixSlice
        where TRight : IMatrixSlice
    {
        for (var i = 0; i < @this.Length; i++)
            @this[i] += other[i] * multConst;
        return @this;
    }
    
    /// <summary>Execute Euler method (RK-1) step: <c>this[i] := y0[i] + dt*f[i]</c></summary>
    public static TLeft EulerStep<TLeft, TRight>(this TLeft @this, double dt, TRight y0, TRight f)
        where TLeft : IMatrixSlice
        where TRight : IMatrixSlice
    {
        for (var i = 0; i < @this.Length; i++)
            @this[i] = y0[i] + dt*f[i];
        return @this;
    }
    
    /// <summary>Execute: <c>this[i] := y0[i] + dt*(yt[i] + this[i])</c></summary>
    public static TLeft RK1ImplicitCombine<TLeft, TRight>(this TLeft @this, double dt, TRight y0, TRight yt)
        where TLeft : IMatrixSlice
        where TRight : IMatrixSlice
    {
        for (var i = 0; i < @this.Length; i++)
            @this[i] = y0[i] + dt*(@this[i] + yt[i]);
        return @this;
    }
    
    /// <summary>Execute RK-2 final step: <c>this[i] := this[i] + dt*(k1[i] + k2[i])</c></summary>
    public static TLeft RK2Combine<TLeft, TRight>(this TLeft @this, double dt, TRight k1, TRight k2)
        where TLeft : IMatrixSlice
        where TRight : IMatrixSlice
    {
        for (var i = 0; i < @this.Length; i++)
            @this[i] += dt*(k1[i] + k2[i]);
        return @this;
    }
    
    /// <summary>Execute RK-4 final step: <c>this[i] := this[i] + dt*(k1[i] + 2*k2[i] + 2*k3[i] + k4[i])</c></summary>
    public static TLeft RK4Combine<TLeft, TRight>(this TLeft @this, double dt, TRight k1, TRight k2, TRight k3, TRight k4)
        where TLeft : IMatrixSlice
        where TRight : IMatrixSlice
    {
        for (var i = 0; i < @this.Length; i++)
            @this[i] += dt*(k1[i] + 2*k2[i] + 2*k3[i] + k4[i]);
        return @this;
    }
    

    /// <summary>Execute: <c>this[i] := slice[i]</c></summary>
    public static void Set<T>(this T @this, T slice) where T : IMatrixSlice 
    {
        var length = Math.Min(@this.Length, slice.Length);
        Array.Copy(slice.AsArray(), @this.AsArray(), length);
    }
    
    /// <summary>Execute: <c>this[i] := fill</c></summary>
    public static void Set<T>(this T @this, double fill) where T : IMatrixSlice 
    {
        Array.Fill(@this.AsArray(), fill);
    }
}