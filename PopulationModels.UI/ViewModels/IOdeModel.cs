using System.ComponentModel;
using PopulationModels.UI.Computing;
using PopulationModels.UI.ViewModels;

namespace PopulationModels.UI.ViewModels;

public interface IOdeModel : INotifyPropertyChanged, INotifyPropertyChanging
{
    public IEnumerable<RangeModelParameter> Parameters { get; }
    public IEnumerable<RangeModelParameter> InitialValues { get; }
    public string Name { get; }
    public string Formula { get; }
    public string GetVariableName(int index);
    public IOdeSystem OdeSystem { get; }
}