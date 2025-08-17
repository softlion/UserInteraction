namespace Vapolia.UserInteraction;

#if !IOS && !MACCATALYST && !ANDROID && !WINDOWS
public partial class UserInteraction
{
    static string CantRunOnNonPlatform => "Please use a platform specific implementation. Can't run on non platform specific code. Use a mock instead."; 

    static Task<bool> PlatformConfirm(string message, string? title, string okButton, string cancelButton, CancellationToken? dismiss)
        => throw new NotSupportedException(CantRunOnNonPlatform);

    static Task<ConfirmThreeButtonsResponse> PlatformConfirmThreeButtons(string message, string? title, string positive, string negative, string neutral)
        => throw new NotSupportedException(CantRunOnNonPlatform);

    static Task PlatformAlert(string message, string title, string okButton)
        => throw new NotSupportedException(CantRunOnNonPlatform);

    static Task<string?> PlatformInput(string message, string? defaultValue, string? placeholder, string? title, string okButton, string cancelButton, FieldType fieldType, int maxLength, bool selectContent)
        => throw new NotSupportedException(CantRunOnNonPlatform);


    static IWaitIndicator PlatformWaitIndicator(CancellationToken dismiss, string? message, string? title, int? displayAfterSeconds, bool userCanDismiss)
        => throw new NotSupportedException(CantRunOnNonPlatform);

    static Task PlatformActivityIndicator(CancellationToken dismiss, double? apparitionDelay, uint? argbColor)
        => throw new NotSupportedException(CantRunOnNonPlatform);

    static Task<int> PlatformMenu(CancellationToken dismiss, bool userCanDismiss, System.Drawing.RectangleF? position, string? title = null, string? description = null, int defaultActionIndex = -1, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons)
        => throw new NotSupportedException(CantRunOnNonPlatform);

    static Task PlatformToast(string text, ToastStyle style = ToastStyle.Notice, ToastDuration duration = ToastDuration.Normal, ToastPosition position = ToastPosition.Bottom, int positionOffset = 20, CancellationToken? dismiss = null)
        => throw new NotSupportedException(CantRunOnNonPlatform);
}
#endif
