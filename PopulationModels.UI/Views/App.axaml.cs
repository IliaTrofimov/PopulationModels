using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PopulationModels.UI.ViewModels;


namespace PopulationModels.UI.Views;

public partial class App : Application
{
    private MainWindow? mainWindow;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            mainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
            
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ShowAboutWindow(object? sender, EventArgs e)
    {
        if (mainWindow != null && !AboutWindow.IsCreated)
            new AboutWindow().ShowDialog(mainWindow);
    }

    private void SetRussianLanguage(object? sender, EventArgs e)
    {
        mainWindow?.SetRussianLang_Click(null, null);
    }
    
    private void SetEnglishLanguage(object? sender, EventArgs e)
    {
        mainWindow?.SetEnglishLang_Click(null, null);
    }
    
}