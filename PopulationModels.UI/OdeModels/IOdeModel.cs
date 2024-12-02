using System.ComponentModel;
using PopulationModels.Computing.OdeSystems;
using PopulationModels.UI.ViewModels.ModelParameter;


namespace PopulationModels.UI.OdeModels;

public class OdeExample
{
    public string Name {get;}
    public string Description {get;}
    
}

public interface IOdeModel : INotifyPropertyChanged, INotifyPropertyChanging
{
    public IReadOnlyList<RangeModelParameter> Parameters { get; }
    
    public IReadOnlyList<RangeModelParameter> InitialValues { get; }
    
    public string Name { get; }
    
    public string Description { get; }
    
    public string Formula { get; }
    
    public IEnumerable<string> Examples { get; }
    
    public IOdeSystem OdeSystem { get; }
    
    public string GetVariableName(int index);
}