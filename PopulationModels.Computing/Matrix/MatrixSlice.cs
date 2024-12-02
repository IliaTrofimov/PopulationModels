namespace PopulationModels.Computing.Matrix;

public abstract class MatrixSlice : IMatrixSlice
{
    /// <summary>Execute Euler method (RK-1) step: <c>this[i] := y0[i] + dt*f[i]</c></summary>
    public MatrixSlice EulerStep(double dt, IReadOnlySlice y0, IReadOnlySlice f)
    {
        for (var i = 0; i < Length; i++)
            this[i] = y0[i] + dt*f[i];
        return this;
    }
    
    /// <summary>Execute: <c>this[i] := y0[i] + dt*(yt[i] + this[i])</c></summary>
    public MatrixSlice RK1ImplicitCombine(double dt, IReadOnlySlice y0, IReadOnlySlice yt)
    {
        for (var i = 0; i < Length; i++)
            this[i] = y0[i] + dt*(this[i] + yt[i]);
        return this;
    }
    
    /// <summary>Execute RK-2 final step: <c>this[i] := this[i] + dt*(k1[i] + k2[i])</c></summary>
    public MatrixSlice RK2Combine(double dt, IReadOnlySlice k1, IReadOnlySlice k2)
    {
        for (var i = 0; i < Length; i++)
            this[i] += dt*(k1[i] + k2[i]);
        return this;
    }
    
    /// <summary>Execute RK-4 final step: <c>this[i] := this[i] + dt*(k1[i] + 2*k2[i] + 2*k3[i] + k4[i])</c></summary>
    public MatrixSlice RK4Combine(double dt, IReadOnlySlice k1, IReadOnlySlice k2, IReadOnlySlice k3, IReadOnlySlice k4)
    {
        for (var i = 0; i < Length; i++)
            this[i] += dt*(k1[i] + 2*k2[i] + 2*k3[i] + k4[i]);
        return this;
    }
    

    /// <summary>Execute: <c>this[i] := slice[i]</c></summary>
    public void Set(IReadOnlySlice slice)
    {
        var length = Math.Min(Length, slice.Length);
        for (var i = 0; i < length; i++)
            this[i] = slice[i];
    }
    
    
    public virtual double[] AsArray() => [];
    
    public abstract void Next();
    
    public abstract void NextCycle();
    
    public abstract double this[int index] { get; set; }
    
    public abstract int Length { get; }
    
    public abstract int Index { get; }
}