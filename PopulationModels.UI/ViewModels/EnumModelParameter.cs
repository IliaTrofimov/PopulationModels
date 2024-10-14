namespace PopulationModels.UI.ViewModels;

public sealed class EnumModelParameter : ModelParameter
{
    private readonly double[] values;
    private int selectedIndex;

    public IEnumerable<double> Values => values;

    public override double Value
    {
        get => _value;
        set => throw new NotImplementedException($"Cannot set value of {nameof(EnumModelParameter)} directly. Use {nameof(SelectedIndex)} property.");
    }

    public int SelectedIndex
    {
        get => selectedIndex;
        set
        {
            if (value < 0 || value >= values.Length)
                throw new IndexOutOfRangeException(nameof(value));
            if (value == selectedIndex) 
                return;    
                
            SetProperty(ref selectedIndex, value);
            OnPropertyChanging(nameof(Value));
            _value = values[value];
            OnPropertyChanged(nameof(Value));
        }
    }


    public EnumModelParameter(string name, double[] availableValues) : this(name, 0, availableValues)
    {
    }
        
    public EnumModelParameter(string name, int selectedIndex, double[] availableValues)
        : base(name, 0)
    {
        if (availableValues == null || availableValues.Length == 0)
            throw new ArgumentNullException(nameof(availableValues));
        if (selectedIndex < 0 || selectedIndex >= availableValues.Length)
            throw new ArgumentOutOfRangeException(nameof(selectedIndex));

        values = availableValues;
        _value = availableValues[selectedIndex];
    }
}