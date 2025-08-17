using Microsoft.Extensions.Logging;

[assembly: XmlnsDefinition("https://vapolia.eu/userinteraction", "Vapolia.UserInteraction")]
[assembly: Microsoft.Maui.Controls.XmlnsPrefix("https://vapolia.eu/userinteraction", "ui")]
[assembly: Microsoft.Maui.Controls.XmlnsPrefix("clr-namespace:Vapolia.UserInteraction;assembly=Vapolia.UserInteraction", "ui")]

namespace Vapolia.UserInteraction;

public static class UserInteractionMauiAppBuilderExtensions
{
    /// <summary>
    /// Add Maui handlers
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="getWindow">A function that returns the current window, where to place popups</param>
    /// <param name="log">An optional logger to log internal issues</param>
    /// <returns></returns>
    public static MauiAppBuilder UseUserInteraction(this MauiAppBuilder builder, Func<Window> getWindow, ILogger<UserInteraction>? log = null)
    {
        UserInteraction.GetWindow = getWindow;
        UserInteraction.Log = log;
        
        return builder;
    }
}