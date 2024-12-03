using PopulationModels.UI.OdeModels;


namespace PopulationModels.UI.ViewModels.ModelParameter;

public class RangeModelParameter : ModelParameter<double>
{
    public double MaxValue { get; }
    public double MinValue { get; }
    public virtual bool IsDisplayable { get; }
    public virtual bool IsPeriodical { get;  }
    
    public override double Value
    {
        get => currentValue;
        set
        {
            if (value > MaxValue || value < MinValue)
            {
                SetValue(DefaultValue);
                throw new ArgumentOutOfRangeException(nameof(Value), $"Parameter '{Name}' should have currentValue in range [{MinValue}, {MaxValue}]. Given currentValue {value}.");
            }
            SetValue(value);
        }
    }
    
    public double TicksStep => Math.Abs(MaxValue - MinValue) / 20;
    
    protected override void SetValue(double newValue)
    {
        if (Math.Abs(newValue - currentValue) <= 5*1e-3) return;
        
        lastValue = currentValue;
        currentValue = newValue;
        
        FirePropertyChangedEvent(new ModelParameterChangedEventArgs(nameof(Value), Name, currentValue, lastValue));
    }

    public RangeModelParameter(string name, double defaultCurrentValue = 0, double minValue = double.NegativeInfinity, double maxValue = double.PositiveInfinity)
        : base(name, defaultCurrentValue)
    {
        if (maxValue < minValue)
            throw new ArgumentException($"MinValue ({minValue}) should be less then MaxValue ({maxValue}).");
        if (defaultCurrentValue > maxValue || defaultCurrentValue < minValue)
            throw new ArgumentOutOfRangeException(nameof(defaultCurrentValue), $"DefaultValue ({defaultCurrentValue}) should be less then MaxValue ({maxValue}) and greater the MinValue ({minValue}).");
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));

        MinValue = minValue;
        MaxValue = maxValue;
        currentValue = defaultCurrentValue;
    }

    public RangeModelParameter() : base("-/-", 0)
    {
    }
}