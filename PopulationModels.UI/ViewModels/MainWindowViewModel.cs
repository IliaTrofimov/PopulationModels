using System.Linq;

namespace PopulationModels.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public EnumModelParameter TimeStep { get; } = new("Δt", 1, [0.1, 0.01, 1e-3, 1e-4, 1e-5, 1e-6]);
    public EnumModelParameter MaxTime { get; } = new("max T", 2, [0.5, 1, 2, 5, 10, 15, 20, 25, 50]);
    

    private int selectedOdeModelIndex;

    private readonly IOdeModel[] odeModels = 
    [
        new BazykinModelA(),
        new LotkaVolterraModel()
    ]; 

    public int SelectedOdeModelIndex
    {
        get => selectedOdeModelIndex;
        set 
        {
            SetProperty(ref selectedOdeModelIndex, value);
            OnPropertyChanging(nameof(SelectedOdeModel));
            OnPropertyChanged(nameof(SelectedOdeModel));
        }
    }

    public IOdeModel SelectedOdeModel => odeModels[selectedOdeModelIndex];

    public IEnumerable<string> KnownOdeModels => odeModels.Select(x => x.Name);
    

    public MainWindowViewModel()
    {
        SetInternalPropertyChangedHandlers(TimeStep, MaxTime);
        SetInternalPropertyChangedHandlers(odeModels);
    }
}