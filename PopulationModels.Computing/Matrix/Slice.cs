using System.Buffers;


namespace PopulationModels.Computing.Matrix;

public sealed class Slice : MatrixSlice, IDisposable
{
    private readonly bool usesArrayPool;
    private readonly double[] data;
    
    public Slice()
    {
        data = [];
        usesArrayPool = false;
        Length = 0;
    }
    
    public Slice(int length, bool? useArrayPool = null)
    {
        data = useArrayPool is true 
            ? ArrayPool<double>.Shared.Rent(length)
            : new double[length];
        usesArrayPool = useArrayPool ?? length >= 256;
        Length = length;
    }
    
    public Slice(int length, double fill, bool useArrayPool = false) 
        : this(length, useArrayPool)
    {
        for (var i = 0; i < length; i++)
            data[i] = fill;
    }
    
    public Slice(int length, double start, double end, bool useArrayPool = false) 
        : this(length, useArrayPool)
    {
        var delta = Math.Abs(end - start) / length;
        for (var i = 0; i < length; i++)
            data[i] = start + i * delta;
    }

    public override double this[int index]
    {
        get => data[index];
        set => data[index] = value;
    }
    
    public override int Length { get; }

    public override int Index => -1;

    public override void Next() {}
    
    public override void NextCycle() {}
    
    public static implicit operator double[](Slice slice) => slice.data;
    
    public override string ToString()
    {
        return Length <= 10 
            ? $"Slice{(usesArrayPool ? "_pooled" : "")}[{Length}] <{string.Join("; ", data.Take(Length).Select(v => $"{v:F3}"))}>" 
            : $"Slice{(usesArrayPool ? "_pooled" : "")}[{Length}] <{string.Join("; ", data.Take(10).Select(v => $"{v:F3}"))}; ... {this[Length - 1]:F3}>";
    }
    
    public void Dispose()
    {
        if (usesArrayPool && Length != 0)
            ArrayPool<double>.Shared.Return(data, clearArray: true);
    }
}