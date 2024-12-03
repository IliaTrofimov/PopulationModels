using PopulationModels.Computing.Matrix;


namespace PopulationModels.Computing.OdeSystems;


/// <summary>Abstract ODE system.</summary>
public interface IOdeSystem
{
    /// <summary>Get derivatives' values: <i>y'(t) = f(t, x1, x2, ...)</i>.</summary>
    /// <param name="t">Time value (independent variable).</param>
    /// <param name="y">Initial values vector.</param>
    /// <param name="result">Output vector <i>result = y' = f(t, x1, x2, ...)</i>.</param>
    public void Derivatives(double t, IReadOnlySlice y, IMatrixSlice result);
}