using PopulationModels.Computing.Matrix;


namespace PopulationModels.Computing.OdeSystems;

/// <summary>
/// Implementation of A. D. Bazykin's model with time dependence of B and D params:<br/>
/// <i>x' = Ax - B(t)*xy / (1 + px) - Ex^2</i><br/>
/// <i>y' = -Cy + D(t)*xy / (1 + px) - Mx^2</i><br/>
/// <i>B(t) = B_0 * sin(pi/dayPeriod * t)</i><br/>
/// <i>D(t) = D_0 * sin(pi/dayPeriod * t)</i><br/>
/// </summary>
public class BazykinSystemWithTime : IOdeSystem
{
    private double a, b, c, d, e, m, p;
    private double dayPeriod;

    public BazykinSystemWithTime(double a, double b, double c, double d, double e, double m, double p, double dayPeriod = 10)
    {
        Set(a, b, c, d, e, m, p, dayPeriod);
    }
    
    public BazykinSystemWithTime Set(double a, double b, double c, double d, double e, double m, double p, double dayPeriod = 10)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        this.e = e;
        this.m = m;
        this.p = p;
        this.dayPeriod = dayPeriod;
        return this;
    }
    
    public void Derivatives(double t, IReadOnlySlice y, IMatrixSlice result)
    {
        var day = Math.Sin(Math.PI / dayPeriod * t) + 1;
        var day_xy_px = day * y[0] * y[1] / (1 + p * y[0]);
        result[0] = a * y[0] - b * day_xy_px - e * Math.Pow(y[0], 2);
        result[1] = -c * y[1] + d * day_xy_px - m * Math.Pow(y[1], 2);
        result[2] = day - 1;
    }
}