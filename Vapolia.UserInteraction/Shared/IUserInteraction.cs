using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vapolia.UserInteraction
{
    /// <summary>
    /// Impact displayed keyboard
    /// </summary>
    public enum FieldType
    {
        Default,
        Email,
        Integer,
        Decimal
    }

    public interface IWaitIndicator
    {
        /// <summary>
        /// cancelled if the indicator is dismissed by the user (if userCanDismiss is true)
        /// </summary>
        CancellationToken UserDismissedToken { get; }

        /// <summary>
        /// Update the title text while displayed
        /// </summary>
        string Title { set; }

        /// <summary>
        /// Update the body text while displayed
        /// </summary>
        string Body { set; }
    }

    public interface IUserInteraction
	{
        /// <summary>
        /// Set default color for all activity indicators
        /// </summary>
	    uint DefaultColor { set; }

        //void Confirm(string message, Action okClicked, string title = null, string okButton = "OK", string cancelButton = "Cancel");
        //void Confirm(string message, Action<bool> answer, string title = null, string okButton = "OK", string cancelButton = "Cancel");
		Task<bool> Confirm(string message, string title = null, string okButton = "OK", string cancelButton = "Cancel", CancellationToken? dismiss = null);

        //void Alert(string message, Action done = null, string title = "", string okButton = "OK");
		Task Alert(string message, string title = "", string okButton = "OK");

	    Task<string?> Input(string message, string defaultValue = null, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", FieldType fieldType = FieldType.Default, int maxLength = 0, bool selectContent = true);

	    void ConfirmThreeButtons(string message, Action<ConfirmThreeButtonsResponse> answer, string title = null, string positive = "Yes", string negative = "No",
	        string neutral = "Maybe");

	    /// <summary>
	    /// Displays a wait indicator (title + body + indeterminate progress bar)
	    /// </summary>
	    /// <param name="dismiss">CancellationToken that dismiss the indicator when cancelled</param>
	    /// <param name="message">body</param>
	    /// <param name="title"></param>
	    /// <param name="displayAfterSeconds">delay show. Can be cancelled before it is displayed.</param>
	    /// <param name="userCanDismiss">Enable tap to dismiss</param>
	    /// <returns>A controller for the wait indicator</returns>
	    IWaitIndicator WaitIndicator(CancellationToken dismiss, string message = null, string title=null, int? displayAfterSeconds = null, bool userCanDismiss = true);

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
        Task<int> Menu(CancellationToken dismiss, bool userCanDismiss, string? title, string description, int defaultActionIndex, string cancelButton, string destroyButton, params string[] otherButtons);
	    
        Task<int> Menu(CancellationToken dismiss, bool userCanDismiss, string? title, string cancelButton, string? destroyButton, params string[] otherButtons);

        /// <summary>
        /// 
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

    public enum ToastStyle
    {
        Custom,
        Info,
        Notice,
        Warning,
        Error
    }

    public enum ToastDuration
    {
        Short = 1000,
        Normal = 2500,
        Long = 8000
    }

    public enum ToastPosition
    {
        Top,
        Middle,
        Bottom
    }
}
