using System.Linq;

using PopulationModels.UI.Models;

namespace PopulationModels.UI.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public EnumModelParameter TimeStep { get; private init; } = new("Δt", 0.1, 0.01, 1e-3, 1e-4, 1e-5, 1e-6);
        public EnumModelParameter MaxTime { get; private init; } = new("max T", 0.5, 1, 2, 5, 10, 20);


        private int selectedOdeModelIndex = 0;

        private readonly IOdeModel[] odeModels = 
        [
            new LotkaVolterraModel(),
            new BazykinModelA()
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
            TimeStep.PropertyChanged += InternalPropertyChanged;
            MaxTime.PropertyChanged += InternalPropertyChanged;
        }
        
        private void InternalPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }
    }
}
