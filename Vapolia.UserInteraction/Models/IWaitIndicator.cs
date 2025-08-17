namespace Vapolia.UserInteraction;

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