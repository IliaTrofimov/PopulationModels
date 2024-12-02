using PopulationModels.Computing.Matrix;


namespace PopulationModels.Computing.OdeSystems;

public interface IOdeSystem
{
    public void Derivatives(double t, IReadOnlySlice y, IMatrixSlice result);
}