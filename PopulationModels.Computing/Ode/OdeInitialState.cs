namespace PopulationModels.Computing.Ode;

public readonly struct OdeInitialState
{
    private const double EPSILON = 1e-10;
    
    public readonly double[] Y0;
    public double Start { get; } = double.NaN;
    public double End { get; } = double.NaN;
    public double Step { get; } = double.NaN;
    public int Steps { get; } = 0;
    public int Variables { get; } 
    public bool IsEmpty { get; } = true;


    public static OdeInitialState FromSteps(double start, double end, int steps, double[] y0)
    {
        return new OdeInitialState(start, end, Math.Abs(end - start) / (steps), y0);
    }

    public OdeInitialState(double start, double end, double step, double[] y0)
    {
        if (step <= 0)
            throw new ArgumentOutOfRangeException(nameof(step), "Step must be greater than zero.");
        if (y0 == null || y0.Length == 0)
            throw new ArgumentException("Y0 must not be null or empty.", nameof(y0));
        
        Start = Math.Min(start, end);
        End = Math.Max(start, end);
        Step = step;
        Steps = (int)(Math.Abs(End - Start) / Step);
        Y0 = y0;
        IsEmpty = false;
        Variables = y0.Length;
    }

    public override string ToString() => $"Range [{Start:F3}: {End:F3}: {Step:E1}], Variables: {Y0.Length}";

    public bool Equals(OdeInitialState other)
    {
        return Math.Abs(other.Step - Step) < EPSILON && 
               Math.Abs(other.End - End) < EPSILON && 
               Math.Abs(other.Start - Start) < EPSILON && 
               Variables == other.Variables &&
               Y0.Zip(other.Y0, (yA, yB) => Math.Abs(yA - yB) < EPSILON).All(equal => equal);
    }

    public override bool Equals(object? obj) => obj is OdeInitialState other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Start, End, Step, Y0);

    public static bool operator ==(OdeInitialState left, OdeInitialState right) => left.Equals(right);

    public static bool operator !=(OdeInitialState left, OdeInitialState right) => !left.Equals(right);
}