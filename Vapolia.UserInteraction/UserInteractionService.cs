namespace Vapolia.UserInteraction;

/// <summary>
/// Implementation of IUserInteraction that delegates to the static UserInteraction methods.
/// This allows for dependency injection while maintaining backward compatibility with static methods.
/// </summary>
internal class UserInteractionService : IUserInteraction
{
    /// <summary>
    /// Set the default color for all activity indicators
    /// In android use global styles instead.
    /// </summary>
    public Color? DefaultColor 
    { 
        get => UserInteraction.DefaultColor; 
        set => UserInteraction.DefaultColor = value; 
    }

    /// <summary>
    /// Shows a confirmation dialog with OK and Cancel buttons.
    /// </summary>
    public Task<bool> Confirm(string message, string? title = null, string okButton = "OK", string cancelButton = "Cancel", CancellationToken? dismiss = null)
        => UserInteraction.Confirm(message, title, okButton, cancelButton, dismiss);

    /// <summary>
    /// Shows an alert dialog with a message and a single button.
    /// </summary>
    public Task Alert(string message, string title = "", string okButton = "OK")
        => UserInteraction.Alert(message, title, okButton);

    /// <summary>
    /// Shows an input dialog for text entry.
    /// </summary>
    public Task<string?> Input(string message, string? defaultValue = null, string? placeholder = null, string? title = null, string okButton = "OK", string cancelButton = "Cancel", FieldType fieldType = FieldType.Default, int maxLength = 0, bool selectContent = true)
        => UserInteraction.Input(message, defaultValue, placeholder, title, okButton, cancelButton, fieldType, maxLength, selectContent);

    /// <summary>
    /// Shows a confirmation dialog with three buttons (positive, negative, neutral).
    /// </summary>
    public Task<ConfirmThreeButtonsResponse> ConfirmThreeButtons(string message, string? title = null, string positive = "Yes", string negative = "No", string neutral = "Maybe")
        => UserInteraction.ConfirmThreeButtons(message, title, positive, negative, neutral);

    /// <summary>
    /// Displays a wait indicator (title + body + indeterminate progress bar)
    /// </summary>
    public IWaitIndicator WaitIndicator(CancellationToken dismiss, string? message = null, string? title = null, int? displayAfterSeconds = null, bool userCanDismiss = true)
        => UserInteraction.WaitIndicator(dismiss, message, title, displayAfterSeconds, userCanDismiss);

    /// <summary>
    /// Display an activity indicator which blocks user interaction.
    /// </summary>
    public Task ActivityIndicator(CancellationToken dismiss, double? apparitionDelay = null, uint? argbColor = null)
        => UserInteraction.ActivityIndicator(dismiss, apparitionDelay, argbColor);

    /// <summary>
    /// Display a single choice menu
    /// </summary>
    public Task<int> Menu(CancellationToken dismiss = default, bool userCanDismiss = true, System.Drawing.RectangleF? position = null, string? title = null, string? description = null, int defaultActionIndex = -1, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons)
        => UserInteraction.Menu(dismiss, userCanDismiss, position, title, description, defaultActionIndex, cancelButton, destroyButton, otherButtons);

    /// <summary>
    /// Shortcut
    /// </summary>
    public Task<int> Menu(CancellationToken dismiss, bool userCanDismiss, string? title = null, string? description = null, int defaultActionIndex = -1, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons)
        => UserInteraction.Menu(dismiss, userCanDismiss, title, description, defaultActionIndex, cancelButton, destroyButton, otherButtons);

    /// <summary>
    /// Shortcut
    /// </summary>
    public Task<int> Menu(string? title = null, string? description = null, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons)
        => UserInteraction.Menu(title, description, cancelButton, destroyButton, otherButtons);

    /// <summary>
    /// Shortcut
    /// </summary>
    public Task<int> Menu(System.Drawing.RectangleF? position = null, string? title = null, string? description = null, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons)
        => UserInteraction.Menu(position, title, description, cancelButton, destroyButton, otherButtons);

    /// <summary>
    /// Display a toast
    /// </summary>
    public Task Toast(string text, ToastStyle style = ToastStyle.Notice, ToastDuration duration = ToastDuration.Normal, ToastPosition position = ToastPosition.Bottom, int positionOffset = 20, CancellationToken? dismiss = null)
        => UserInteraction.Toast(text, style, duration, position, positionOffset, dismiss);
}
