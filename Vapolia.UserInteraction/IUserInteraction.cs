namespace Vapolia.UserInteraction;

/// <summary>
/// Interface for user interaction services providing dialogs, alerts, menus, and toast notifications.
/// This interface can be injected via dependency injection as an alternative to static methods.
/// </summary>
public interface IUserInteraction
{
    /// <summary>
    /// Set the default color for all activity indicators
    /// In android use global styles instead.
    /// </summary>
    Color? DefaultColor { get; set; }

    /// <summary>
    /// Shows a confirmation dialog with OK and Cancel buttons.
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="title">Optional title for the dialog</param>
    /// <param name="okButton">Text for the OK button (default: "OK")</param>
    /// <param name="cancelButton">Text for the Cancel button (default: "Cancel")</param>
    /// <param name="dismiss">Optional cancellation token to dismiss the dialog programmatically</param>
    /// <returns>True if OK was pressed, false if Cancel was pressed or dialog was dismissed</returns>
    Task<bool> Confirm(string message, string? title = null, string okButton = "OK", string cancelButton = "Cancel", CancellationToken? dismiss = null);

    /// <summary>
    /// Shows an alert dialog with a message and a single button.
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="title">Optional title for the dialog</param>
    /// <param name="okButton">Text for the OK button (default: "OK")</param>
    /// <returns>A task that completes when the dialog is dismissed</returns>
    Task Alert(string message, string title = "", string okButton = "OK");

    /// <summary>
    /// Shows an input dialog for text entry.
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="defaultValue">Default text value</param>
    /// <param name="placeholder">Placeholder text</param>
    /// <param name="title">Optional title for the dialog</param>
    /// <param name="okButton">Text for the OK button (default: "OK")</param>
    /// <param name="cancelButton">Text for the Cancel button (default: "Cancel")</param>
    /// <param name="fieldType">Type of input field (Default, Email, Integer, Decimal)</param>
    /// <param name="maxLength">Maximum length of input (0 for no limit)</param>
    /// <param name="selectContent">Whether to select all text when dialog opens</param>
    /// <returns>The entered text if OK was pressed, null if Cancel was pressed or dialog was dismissed</returns>
    Task<string?> Input(string message, string? defaultValue = null, string? placeholder = null, string? title = null, string okButton = "OK", string cancelButton = "Cancel", FieldType fieldType = FieldType.Default, int maxLength = 0, bool selectContent = true);

    /// <summary>
    /// Shows a confirmation dialog with three buttons (positive, negative, neutral).
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="title">Optional title for the dialog</param>
    /// <param name="positive">Text for the positive button (default: "Yes")</param>
    /// <param name="negative">Text for the negative button (default: "No")</param>
    /// <param name="neutral">Text for the neutral button (default: "Maybe")</param>
    /// <returns>The response indicating which button was pressed</returns>
    Task<ConfirmThreeButtonsResponse> ConfirmThreeButtons(string message, string? title = null, string positive = "Yes", string negative = "No", string neutral = "Maybe");

    /// <summary>
    /// Displays a wait indicator (title + body + indeterminate progress bar)
    /// </summary>
    /// <param name="dismiss">CancellationToken that dismiss the indicator when cancelled</param>
    /// <param name="message">body</param>
    /// <param name="title"></param>
    /// <param name="displayAfterSeconds">delay show. Can be cancelled before it is displayed.</param>
    /// <param name="userCanDismiss">Enable tap to dismiss</param>
    /// <returns>A controller for the wait indicator</returns>
    IWaitIndicator WaitIndicator(CancellationToken dismiss, string? message = null, string? title = null, int? displayAfterSeconds = null, bool userCanDismiss = true);

    /// <summary>
    /// Display an activity indicator which blocks user interaction.
    /// </summary>
    /// <param name="dismiss">cancel this token to dismiss the activity indicator</param>
    /// <param name="apparitionDelay">show indicator after this delay. The user interaction is not disabled during this delay: this may be an issue.</param>
    /// <param name="argbColor">activity indicator tint</param>
    /// <returns></returns>
    Task ActivityIndicator(CancellationToken dismiss, double? apparitionDelay = null, uint? argbColor = null);

    /// <summary>
    /// Display a single choice menu
    /// </summary>
    /// <param name="dismiss">optional. Can be used to close the menu programatically.</param>
    /// <param name="userCanDismiss">true to allow the user to close the menu using a hardware key.</param>
    /// <param name="position">optional absolute position on screen</param>
    /// <param name="title">optional title</param>
    /// <param name="description">optional description</param>
    /// <param name="defaultActionIndex">from 2 to 2+number of actions. Otherwise ignored.</param>
    /// <param name="cancelButton">optional cancel button. If null the cancel button is not shown.</param>
    /// <param name="destroyButton">optional destroy button. Will be red.</param>
    /// <param name="otherButtons">If a button is null, the index are still incremented, but the button won't appear</param>
    /// <returns>
    /// A task which completes when the menu has disappeared
    /// 0: cancel button or hardware key is pressed
    /// 1: destroy button is pressed
    /// 2-n: other matching button is pressed
    /// </returns>
    /// <remarks>
    /// If otherButtons is null, the indexes are still incremented, but the button won't appear. 
    /// This enables easy scenario where the otherButtons array is changing between calls.
    /// </remarks>
    Task<int> Menu(CancellationToken dismiss = default, bool userCanDismiss = true, System.Drawing.RectangleF? position = null, string? title = null, string? description = null, int defaultActionIndex = -1, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons);

    /// <summary>
    /// Shortcut
    /// </summary>
    Task<int> Menu(CancellationToken dismiss, bool userCanDismiss, string? title = null, string? description = null, int defaultActionIndex = -1, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons);

    /// <summary>
    /// Shortcut
    /// </summary>
    Task<int> Menu(string? title = null, string? description = null, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons);

    /// <summary>
    /// Shortcut
    /// </summary>
    Task<int> Menu(System.Drawing.RectangleF? position = null, string? title = null, string? description = null, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons);

    /// <summary>
    /// Display a toast
    /// </summary>
    /// <param name="text"></param>
    /// <param name="style"></param>
    /// <param name="duration"></param>
    /// <param name="position"></param>
    /// <param name="positionOffset"></param>
    /// <param name="dismiss">optional. Can be used to close the toast programatically.</param>
    /// <returns>A task which completes when the toast has disappeared</returns>
    Task Toast(string text, ToastStyle style = ToastStyle.Notice, ToastDuration duration = ToastDuration.Normal, ToastPosition position = ToastPosition.Bottom, int positionOffset = 20, CancellationToken? dismiss = null);
}
