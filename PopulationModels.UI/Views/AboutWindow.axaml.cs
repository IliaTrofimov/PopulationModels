using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using PopulationModels.Computing.Matrix;
using PopulationModels.Computing.Ode;
using PopulationModels.UI.OdeModels;
using PopulationModels.UI.Plotting;
using PopulationModels.UI.Resources;
using PopulationModels.UI.ViewModels;
using ScottPlot;
using Label = Avalonia.Controls.Label;


namespace PopulationModels.UI.Views;

public partial class AboutWindow : Window
{
    public static bool IsCreated { get; private set; }

    public AboutWindow()
    {
        if (IsCreated)
            throw new InvalidOperationException("This window is already open.");
        InitializeComponent();
    }

    private void Closed_Click(object? sender, EventArgs e)
    {
        IsCreated = false;
    }
}