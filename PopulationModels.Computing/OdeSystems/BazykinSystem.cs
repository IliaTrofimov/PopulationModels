using PopulationModels.Computing.Matrix;

namespace PopulationModels.Computing.OdeSystems;


/// <summary>
/// Implementation of A. D. Bazykin's model:<br/>
/// <i>x' = Ax - Bxy / (1 + px) - Ex^2</i><br/>
/// <i>y' = -Cy + Dxy / (1 + px) - Mx^2</i>
/// </summary>
public class BazykinSystem : IOdeSystem
{
    private double a, b, c, d, e, m, p;

    public BazykinSystem(double a, double b, double c, double d, double e, double m, double p)
    {
        Set(a, b, c, d, e, m, p);
    }
    
    public BazykinSystem Set(double a, double b, double c, double d, double e, double m, double p)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        this.e = e;
        this.m = m;
        this.p = p;
        return this;
    }
    
    public void Derivatives(double t, IReadOnlySlice y, IMatrixSlice result)
    {
        var xy_px = y[0] * y[1] / (1 + p * y[0]);
        result[0] = a * y[0] - b * xy_px - e * Math.Pow(y[0], 2);
        result[1] = -c * y[1] + d * xy_px - m * Math.Pow(y[1], 2);
    }
}