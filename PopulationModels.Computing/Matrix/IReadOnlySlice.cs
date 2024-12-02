namespace PopulationModels.Computing.Matrix;

public interface IReadOnlySlice
{
    public double this[int index] { get; }
    
    public int Length { get; }
    
    public int Index { get; }
}