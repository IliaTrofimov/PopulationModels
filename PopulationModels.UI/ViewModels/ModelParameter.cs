using CommunityToolkit.Mvvm.ComponentModel;

namespace PopulationModels.UI.ViewModels;

public abstract class ModelParameter : ObservableObject
{
    protected double _value;
        
    public string Name { get; }
    public string? Description { get; set; }
    public virtual double Value { get => _value; set => SetProperty(ref _value, value); }

        
    protected ModelParameter(string name, double defaultValue)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));
        Name = name;
        _value = defaultValue;
    }

        
    public override string ToString() => $"{Name}: {_value}";

    public static implicit operator double(ModelParameter p) => p._value;
}