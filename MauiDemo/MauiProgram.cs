using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Vapolia.UserInteraction;

namespace MauiDemo;

public static class MauiProgram
{
#if DEBUG && WINDOWS
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    public static extern bool AllocConsole();
#endif
    
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
#if DEBUG && WINDOWS
        AllocConsole();
        
        // Redirect Console.WriteLine to that console
        Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
        Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
#endif        
        
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseUserInteraction(() => Application.Current!.Windows[0]);

        builder.Logging.AddDebug();
        builder.Logging.AddSimpleConsole(options =>
        {
            options.ColorBehavior = LoggerColorBehavior.Enabled;
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        });
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            var ex = e.ExceptionObject as Exception;
            Console.WriteLine("AppDomain UnhandledException");
            Console.WriteLine(ex.ToString());
        };
        
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            //LogCrash("TaskScheduler.UnobservedTaskException", e.Exception);
            //e.SetObserved(); // Empêche l’app de crasher
            Console.WriteLine("TaskScheduler UnobservedTaskException");
            Console.WriteLine(e.ToString());
        };

#if WINDOWS
        Microsoft.UI.Xaml.Application.Current.UnhandledException += (sender, e) =>
        {
            Console.WriteLine("WinUI UnhandledException");
            Console.WriteLine(e.ToString());
        };
#endif
        
        return builder.Build();
    }
}