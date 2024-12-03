namespace PopulationModels.UI.ViewModels.ModelParameter;

public class SelectableModelParameter<T> : ModelParameter<T>, ISelectableModelParameter
    where T: notnull
{
    private readonly T[] values;
    private int selectedIndex;

    public IEnumerable<T> Values => values;

    public override T Value
    {
        get => currentValue;
        set => throw new NotImplementedException($"Cannot set currentValue of {nameof(SelectableModelParameter<T>)} directly. Use {nameof(SelectedIndex)} property.");
    }

    public int SelectedIndex
    {
        get => selectedIndex;
        set
        {
            if (value < 0 || value >= values.Length)
                value = 0;
            if (value == selectedIndex) 
                return;

            selectedIndex = value;
            SetValue(values[value]);
        }
    }
    

    public SelectableModelParameter(string name, T[] availableValues)
        : this(name, 0, availableValues)
    {
    }
        
    public SelectableModelParameter(string name, int selectedIndex, T[] availableValues)
        : base(name, default)
    {
        if (availableValues == null || availableValues.Length == 0)
            throw new ArgumentNullException(nameof(availableValues));
        if (selectedIndex < 0 || selectedIndex >= availableValues.Length)
            throw new ArgumentOutOfRangeException(nameof(selectedIndex));

        values = availableValues;
        currentValue = availableValues[selectedIndex];
        this.selectedIndex = selectedIndex;
    }


    public bool TrySetValue(T value)
    {
        var index = Array.IndexOf(values, value);
        if (index < 0) return false;

        if (index != selectedIndex)
            SelectedIndex = index;

        return true;
    }
}