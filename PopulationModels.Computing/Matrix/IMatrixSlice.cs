namespace PopulationModels.Computing.Matrix;

public interface IMatrixSlice : IReadOnlySlice
{
    public new double this[int index] { get; set; }

    public void Next();
    
    public void NextCycle();

    public double[] AsArray();
}