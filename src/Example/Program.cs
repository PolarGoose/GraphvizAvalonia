using Avalonia;
using GraphvizAvalonia;
using ReactiveUI.Avalonia;

namespace Example;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        GraphvizBinariesLocation.Set($"{AppDomain.CurrentDomain.BaseDirectory}/GraphvizBinaries");

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
