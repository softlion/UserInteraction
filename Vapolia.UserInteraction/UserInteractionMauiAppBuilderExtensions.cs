using Microsoft.Extensions.Logging;

[assembly: XmlnsDefinition("https://vapolia.eu/userinteraction", "Vapolia.UserInteraction")]
[assembly: Microsoft.Maui.Controls.XmlnsPrefix("https://vapolia.eu/userinteraction", "ui")]
[assembly: Microsoft.Maui.Controls.XmlnsPrefix("clr-namespace:Vapolia.UserInteraction;assembly=Vapolia.UserInteraction", "ui")]

namespace Vapolia.UserInteraction;

public static class UserInteractionMauiAppBuilderExtensions
{
    /// <summary>
    /// Add Maui handlers and register IUserInteraction service for dependency injection
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="getWindow">A function that returns the current window, where to place popups</param>
    /// <param name="log">An optional logger to log internal issues</param>
    /// <returns></returns>
    public static MauiAppBuilder UseUserInteraction(this MauiAppBuilder builder, Func<Window> getWindow, ILogger<UserInteraction>? log = null)
    {
        UserInteraction.GetWindow = getWindow;
        UserInteraction.Log = log;

        // Register IUserInteraction service for dependency injection
        builder.Services.AddSingleton<IUserInteraction, UserInteractionService>();

        return builder;
    }
}