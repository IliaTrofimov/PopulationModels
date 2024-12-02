using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using PopulationModels.UI.Views;


namespace PopulationModels.UI;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        TaskScheduler.UnobservedTaskException += (sender, ex) =>
        {
            Debug.WriteLine($"Unobserved task exception:\n[SENDER] {sender?.GetType()}\n[MESSAGE]{ex.Exception}");
        };
            
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}