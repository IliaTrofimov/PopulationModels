using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using PopulationModels.UI.ViewModels.ModelParameter;


namespace PopulationModels.UI.Views;

public partial class RangeModelParameterView : UserControl
{
    public static readonly StyledProperty<RangeModelParameter> ParameterProperty =
        AvaloniaProperty.Register<RangeModelParameterView, RangeModelParameter>(nameof(Parameter), new RangeModelParameter());
    
    public RangeModelParameter Parameter { get; set; } = new();
    
    public RangeModelParameterView()
    {
        InitializeComponent();
    }
}