using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vapolia.UserInteraction
{
    public partial class UserInteraction
    {
        internal static uint PlatformDefaultColor { get; set; }

        internal static Task<bool> PlatformConfirm(string message, string? title, string okButton, string cancelButton, CancellationToken? dismiss)
            => throw new NotImplementedException();

	    internal static Task<ConfirmThreeButtonsResponse> PlatformConfirmThreeButtons(string message, string? title, string positive, string negative, string neutral)
            => throw new NotImplementedException();

		internal static Task PlatformAlert(string message, string title, string okButton)
            => throw new NotImplementedException();

	    internal static Task<string?> PlatformInput(string message, string? defaultValue, string? placeholder, string? title, string okButton, string cancelButton, FieldType fieldType, int maxLength, bool selectContent)
            => throw new NotImplementedException();


	    internal static IWaitIndicator PlatformWaitIndicator(CancellationToken dismiss, string? message, string? title, int? displayAfterSeconds, bool userCanDismiss)
            => throw new NotImplementedException();

        internal static Task PlatformActivityIndicator(CancellationToken dismiss, double? apparitionDelay, uint? argbColor)
            => throw new NotImplementedException();

        internal static Task<int> PlatformMenu(CancellationToken dismiss, bool userCanDismiss, System.Drawing.RectangleF? position, string? title = null, string? description = null, int defaultActionIndex = -1, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons)
            => throw new NotImplementedException();

        internal static Task PlatformToast(string text, ToastStyle style = ToastStyle.Notice, ToastDuration duration = ToastDuration.Normal, ToastPosition position = ToastPosition.Bottom, int positionOffset = 20, CancellationToken? dismiss = null)
            => throw new NotImplementedException();
    }
}
