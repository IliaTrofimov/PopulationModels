using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PopulationModels.UI.ViewModels;

public class ViewModelBase : ObservableObject
{
    protected void InternalPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e);
    }

    protected void SetInternalPropertyChangedHandlers(params INotifyPropertyChanged[] properties)
    {
        foreach (var prop in properties)
            prop.PropertyChanged += InternalPropertyChanged;
    }
    
    protected void SetInternalPropertyChangedHandlers(IEnumerable<INotifyPropertyChanged> properties)
    {
        foreach (var prop in properties)
            prop.PropertyChanged += InternalPropertyChanged;
    }
}



