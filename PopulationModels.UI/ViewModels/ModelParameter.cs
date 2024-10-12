namespace PopulationModels.UI.ViewModels
{
    public class ModelParameter : BaseModelParameter
    {
        public double MaxValue { get; private init; }
        public double MinValue { get; private init; }

        public override double Value
        {
            get => _value;
            set
            {
                if (value > MaxValue || value < MinValue)
                {
                    SetProperty(ref _value, defaultValue);
                    throw new ArgumentOutOfRangeException(nameof(Value), $"Parameter '{Name}' should have value in range [{MinValue}, {MaxValue}]. Given value {value}.");
                }

                SetProperty(ref _value, value);
            }
        }

        public ModelParameter(string name, double defaultValue = 0, double minValue = double.NegativeInfinity, double maxValue = double.PositiveInfinity)
            : base(name, defaultValue)
        {
            if (maxValue < minValue)
                throw new ArgumentException($"MinValue ({minValue}) should be less then MaxValue ({maxValue}).");
            if (defaultValue > maxValue || defaultValue < minValue)
                throw new ArgumentOutOfRangeException(nameof(defaultValue), $"DefaultValue ({defaultValue}) should be less then MaxValue ({maxValue}) and greater the MinValue ({minValue}).");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            MinValue = minValue;
            MaxValue = maxValue;
            Value = defaultValue;
        }
    }

}
