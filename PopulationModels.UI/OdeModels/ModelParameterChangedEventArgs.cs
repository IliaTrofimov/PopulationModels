using System.ComponentModel;


namespace PopulationModels.UI.OdeModels;

public class ModelParameterChangedEventArgs : PropertyChangedEventArgs
{
    private const string DESCRIPTION_FORMAT_SHORT = "{0}: {1:F3} >> {2:F3}";
    private const string DESCRIPTION_FORMAT = "{0}: {1:F3} >> {2:F3}\n{3}";
    
    public string ParameterName { get; }
    public string Description { get; }
    public double OldValue { get; }
    public double NewValue { get; }
    
    
    public ModelParameterChangedEventArgs(ModelParameterChangedEventArgs args, string description) 
        : this(args.PropertyName, args.ParameterName, args.NewValue, args.OldValue, description)
    {
    }
    
    public ModelParameterChangedEventArgs(string? propertyName, string parameterName, double newValue, double oldValue, string description = "") 
        : base(propertyName)
    {
        OldValue = oldValue;
        NewValue = newValue;
        ParameterName = parameterName;
        Description = string.IsNullOrEmpty(description)
            ? string.Format(DESCRIPTION_FORMAT_SHORT, parameterName, newValue, oldValue)
            : string.Format(DESCRIPTION_FORMAT, parameterName, newValue, oldValue, description);
    }
}