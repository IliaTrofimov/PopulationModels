using System.ComponentModel;
using PopulationModels.UI.OdeModels;


namespace PopulationModels.UI.ViewModels.ModelParameter;



public abstract class ModelParameter<T> : INotifyPropertyChanged
{
    public delegate void ValueChangedHandler(string name, T newValue, T oldValue);
    
    protected T currentValue;
    protected T lastValue;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name { get; }
    public virtual string? Description { get; }
    public virtual T Value
    {
        get => currentValue; 
        set => SetValue(value);
    }
    
    public T DefaultValue { get; protected set; }
        
    protected ModelParameter(string name, T defaultCurrentValue)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));
        Name = name;
        currentValue = defaultCurrentValue;
        lastValue = defaultCurrentValue;
        DefaultValue = defaultCurrentValue;
    }
    
    
    public static implicit operator T(ModelParameter<T> p) => p.currentValue;
    
    public virtual void Reset() => Value = DefaultValue;

    protected virtual void SetValue(T newValue)
    {
        if (newValue?.Equals(currentValue) == true) return;
        
        lastValue = currentValue;
        currentValue = newValue;
        FirePropertyChangedEvent(new PropertyChangedEventArgs(nameof(Value)));
    }
    
    protected void FirePropertyChangedEvent(PropertyChangedEventArgs eventArgs)
    {
        PropertyChanged?.Invoke(this, eventArgs);
    }
    
    public override string ToString() => $"{Name}: {currentValue}";
}