using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.UI.Popups;
using Button = Microsoft.UI.Xaml.Controls.Button;
using HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment;
using VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment;
using Window = Microsoft.UI.Xaml.Window;

namespace Vapolia.UserInteraction;

public partial class UserInteraction
{
    private static Color? defaultColor;

    internal static uint PlatformDefaultColor { set => defaultColor = FromArgb(value); }

    static Color FromArgb(uint value)
        => new Color((value >> 16 & 0xff)/255f, (value >> 8 & 0xff)/255f, (value & 0xff)/255f, (value >> 24 & 0xff)/255f);

    internal static Task<bool> PlatformConfirm(string message, string? title = null, string okButton = "OK", string cancelButton = "Cancel", CancellationToken? dismiss = null)
    {
        throw new NotSupportedException();

        /*
        var currentView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().;
        
        var xamlRoot = Windows.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(currentView).XamlRoot;


        var tcs = new TaskCompletionSource<bool>();
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            //Window.Current.Content.XamlRoot null

            var confirm = new ContentDialog
            {
                Title = title,
                Content = message,
                
                CloseButtonText = cancelButton,
                CloseButtonCommand = new Command(() => { tcs.TrySetResult(false); }),
                
                IsPrimaryButtonEnabled = true,
                PrimaryButtonCommand = new Command(() => { tcs.TrySetResult(true); }),
                PrimaryButtonText = okButton,

                DefaultButton = ContentDialogButton.Close,
                XamlRoot = xamlRoot
            };


            if (dismiss != null)
            {
                var registration = dismiss.Value.Register(() =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        confirm.Hide();
                        tcs.TrySetResult(false);
                    });
                });
                _ = tcs.Task.ContinueWith(t => registration.Dispose());
            }
        
            await confirm.ShowAsync(ContentDialogPlacement.Popup);
            var i = 0;
        });

        return tcs.Task;
        */
    }

    internal static Task<ConfirmThreeButtonsResponse> PlatformConfirmThreeButtons(string message, string? title = null, string positive = "Yes", string negative = "No", string neutral = "Maybe")
    {
        throw new NotImplementedException();
        //    var tcs = new TaskCompletionSource<ConfirmThreeButtonsResponse>();
        //    MainThread.BeginInvokeOnMainThread(() =>
        //    {
        //        var confirm = new UIAlertView(title ?? string.Empty, message, (IUIAlertViewDelegate?)null, negative, positive, neutral);
        //        confirm.Clicked +=
        //            (sender, args) =>
        //            {
        //                var buttonIndex = args.ButtonIndex;
        //                if (buttonIndex == confirm.CancelButtonIndex)
        //                    tcs.TrySetResult(ConfirmThreeButtonsResponse.Negative);
        //                else if (buttonIndex == confirm.FirstOtherButtonIndex)
        //                    tcs.TrySetResult(ConfirmThreeButtonsResponse.Positive);
        //                else
        //                    tcs.TrySetResult(ConfirmThreeButtonsResponse.Neutral);
        //            };
        //        confirm.Show();
        //    });
        //    return tcs.Task;
    }

    internal static Task PlatformAlert(string message, string title = "", string okButton = "OK")
    {
        throw new NotImplementedException();
        //var tcs = new TaskCompletionSource<bool>();
        //MainThread.BeginInvokeOnMainThread(() =>
        //{
        //    var alert = new UIAlertView(title ?? string.Empty, message, (IUIAlertViewDelegate?)null, okButton);
        //    alert.Clicked += (sender, args) => tcs.TrySetResult(true);
        //    alert.Show();
        //});
        //return tcs.Task;
    }

    internal static Task<string?> PlatformInput(string message, string? defaultValue = null, string? placeholder = null, string? title = null, string okButton = "OK", string cancelButton = "Cancel", FieldType fieldType = FieldType.Default, int maxLength = 0, bool selectContent = true)
    {
        throw new NotImplementedException();

        //var tcs = new TaskCompletionSource<string?>();

        //void ConfigureTextField(UITextField textField)
        //{
        //    if (placeholder != null)
        //        textField.Placeholder = placeholder;
        //    if (defaultValue != null)
        //        textField.Text = defaultValue;

        //    if(selectContent && !String.IsNullOrWhiteSpace(defaultValue))
        //    {
        //        textField.EditingDidBegin += (sender, args) =>
        //        {
        //            textField.SelectedTextRange = textField.GetTextRange(textField.BeginningOfDocument, textField.EndOfDocument);
        //            textField.BecomeFirstResponder();
        //        };
        //    }

        //    if (fieldType != FieldType.Default)
        //    {
        //        if (fieldType == FieldType.Email)
        //            textField.KeyboardType = UIKeyboardType.EmailAddress;
        //        else if (fieldType == FieldType.Integer)
        //        {
        //            textField.KeyboardType = UIKeyboardType.NumberPad;
        //            textField.ValueChanged += (sender, args) =>
        //            {
        //                var text = textField.Text ?? "";
        //                var newText = Regex.Replace(text, "[^0-9]", "");
        //                if (text != newText)
        //                    textField.Text = newText;
        //            };
        //        }
        //        else if (fieldType == FieldType.Decimal)
        //        {
        //            textField.KeyboardType = UIKeyboardType.DecimalPad;
        //            textField.ValueChanged += (sender, args) =>
        //            {
        //                var text = textField.Text ?? "";
        //                var newText = Regex.Replace(text, "[^0-9.,\\s]", "");
        //                if (text != newText)
        //                    textField.Text = newText;
        //            };
        //        }
        //    }

        //    if (maxLength > 0)
        //    {
        //        textField.ValueChanged += (sender, args) =>
        //        {
        //            var text = textField.Text;
        //            if (text?.Length > maxLength)
        //                textField.Text = text.Substring(0, maxLength);
        //        };
        //    }
        //}

            
        //var presentingVc = Platform.GetCurrentUIViewController();
        //if (presentingVc != null)
        //{
        //    MainThread.BeginInvokeOnMainThread(() =>
        //    {
        //        var alert = UIAlertController.Create(title ?? string.Empty, message, UIAlertControllerStyle.Alert);
        //        alert.AddAction(UIAlertAction.Create(okButton, UIAlertActionStyle.Default, action => tcs.TrySetResult(alert.TextFields[0].Text)));
        //        alert.AddAction(UIAlertAction.Create(cancelButton, UIAlertActionStyle.Cancel, action => tcs.TrySetResult(null)));
        //        alert.AddTextField(ConfigureTextField);
        //        presentingVc.PresentViewController(alert, true, null);
        //    });
        //}
        //else
        //    Log?.LogWarning("Input: no window/nav controller on which to display");

        //return tcs.Task;
    }

    //class WaitIndicatorImpl : IWaitIndicator
    //{
    //    private string? title, body;

    //    public UIAlertController? Dialog { get; set; }

    //    public WaitIndicatorImpl(CancellationToken userDismissedToken)
    //    {
    //        UserDismissedToken = userDismissedToken;
    //    }

    //    public CancellationToken UserDismissedToken { get; }
    //    public string? Title { set { title = value; if(Dialog!=null) UIApplication.SharedApplication.InvokeOnMainThread(() => Dialog.Title = value); } get => title; }
    //    public string? Body { set { body = value; if (Dialog != null) UIApplication.SharedApplication.InvokeOnMainThread(() => Dialog.Message = value); } get => body; }
    //}

    /// <summary>
    /// </summary>
    /// <param name="dismiss">CancellationToken that dismiss the indicator when cancelled</param>
    /// <param name="message">body</param>
    /// <param name="title"></param>
    /// <param name="displayAfterSeconds">delay show. Can be cancelled before it is displayed.</param>
    /// <param name="userCanDismiss">Enable tap to dismiss</param>
    /// <returns>CancellationToken is cancelled if the indicator is dismissed by the user (if userCanDismiss is true)</returns>
    /// <remarks>
    /// This should block the UI if userCanDismiss is false
    /// </remarks>
    static IWaitIndicator PlatformWaitIndicator(CancellationToken dismiss, string? message = null, string? title=null, int? displayAfterSeconds = null, bool userCanDismiss = true)
    {
        throw new NotImplementedException();

        //var userDismissed = new CancellationTokenSource();
        //var wi = new WaitIndicatorImpl(userDismissed.Token)
        //{
        //    Title = title,
        //    Body = message
        //};
        
        //if(displayAfterSeconds is null or 0)
        //    Do();
        //else
        //{
        //    try
        //    {
        //        Task.Delay(displayAfterSeconds.Value*1000, dismiss).ContinueWith(_ => Do(), TaskContinuationOptions.OnlyOnRanToCompletion);
        //    }
        //    catch (TaskCanceledException)
        //    {
        //    }
        //}
        
        //return wi;

        //void Do()
        //{
        //    MainThread.BeginInvokeOnMainThread(() =>
        //    {
        //        var alert = UIAlertController.Create(title ?? string.Empty, message, UIAlertControllerStyle.Alert);
        //        wi.Dialog = alert;
        //        if (userCanDismiss)
        //            alert.AddAction(UIAlertAction.Create("X", UIAlertActionStyle.Cancel, _ => userDismissed.Cancel()));

        //        dismiss.Register(() => MainThread.BeginInvokeOnMainThread(() =>
        //        {
        //            alert.DismissViewController(true, null);
        //        }), true);

        //        var presentingVc = Platform.GetCurrentUIViewController();
        //        if (presentingVc != null)
        //            MainThread.BeginInvokeOnMainThread(() =>
        //            {
        //                presentingVc.PresentViewController(alert, true, null);
        //            });
        //        else
        //            Log?.LogWarning("Input: no window/nav controller on which to display");
        //    });
        //}
    }

    static Task PlatformActivityIndicator(CancellationToken dismiss, double? apparitionDelay = null, uint? argbColor = null)
    {
        throw new NotImplementedException();

        //var presentingVc = Platform.GetCurrentUIViewController();
        //if (presentingVc == null)
        //{
        //    Log?.LogWarning("UserInteraction.ActivityIndicator: no window on which to display");
        //    return Task.CompletedTask;
        //}

        //var tcs = new TaskCompletionSource<int>();

        //Task.Delay((int)((apparitionDelay ?? 0)*1000+.5), dismiss).ContinueWith(t =>
        //{
        //    if (t.IsCompleted)
        //    {
        //        MainThread.BeginInvokeOnMainThread(() =>
        //        {
        //            var currentView = presentingVc?.View;

        //            var waitView = new UIView {Alpha = 0};
        //            var overlay = new UIView {BackgroundColor = UIColor.White, Alpha = 0.7f};
        //            var indicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Large) {HidesWhenStopped = true};
        //            if (argbColor.HasValue || defaultColor != null)
        //                indicator.Color = argbColor.HasValue ? FromArgb(argbColor.Value) : defaultColor;

        //            waitView.Add(overlay);
        //            waitView.Add(indicator);
        //            currentView?.Add(waitView);

        //            waitView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
        //            waitView.AddConstraints(
        //                overlay.AtTopOf(waitView),
        //                overlay.AtLeftOf(waitView),
        //                overlay.AtBottomOf(waitView),
        //                overlay.AtRightOf(waitView),

        //                indicator.WithSameCenterX(waitView),
        //                indicator.WithSameCenterY(waitView),
        //                indicator.Width().EqualTo(60),
        //                indicator.Height().EqualTo(60)
        //            );

        //            waitView.TranslatesAutoresizingMaskIntoConstraints = false;
        //            currentView?.AddConstraints(
        //                waitView.AtTopOf(currentView),
        //                waitView.AtLeftOf(currentView),
        //                waitView.AtRightOf(currentView),
        //                waitView.AtBottomOf(currentView)
        //            );

        //            UIView.Animate(0.4, () => { waitView.Alpha = 1; });
        //            indicator.StartAnimating();

        //            var registration = dismiss.Register(() => MainThread.BeginInvokeOnMainThread(() =>
        //            {
        //                indicator.StopAnimating();
        //                waitView.RemoveFromSuperview();
        //                waitView.Dispose();
        //                tcs.TrySetResult(0);
        //            }), true);
        //            // ReSharper disable once MethodSupportsCancellation
        //            tcs.Task.ContinueWith(tt => registration.Dispose());
        //        });
        //    }
        //    else
        //        tcs.TrySetResult(0);
        //});
            
        //return tcs.Task;
    }

    /// <summary>
    /// Displays a system menu.
    /// If otherButtons is null, the indexes are still incremented, but the button won't appear. 
    /// This enables easy scenario where the otherButtons array is changing between calls.
    /// </summary>
    /// <param name="dismiss"></param>
    /// <param name="userCanDismiss">NOT SUPPORTED ON IOS</param>
    /// <param name="position">optional: position from top left of screen</param>
    /// <param name="title"></param>
    /// <param name="description">optional</param>
    /// <param name="defaultActionIndex">from 2 to 2+number of actions. Otherwise ignored.</param>
    /// <param name="cancelButton">optional</param>
    /// <param name="destroyButton">optional</param>
    /// <param name="otherButtons">If a button is null, the index are still incremented, but the button won't appear</param>
    /// <returns>
    /// Button indexes:
    /// cancel: 0
    /// destroy: 1
    /// others: 2+index
    /// </returns>
    internal static Task<int> PlatformMenu(CancellationToken dismiss, bool userCanDismiss,
        System.Drawing.RectangleF? position = null,
        string? title = null, string? description = null, int defaultActionIndex = -1, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons)
    {
        throw new NotImplementedException();

        //var presentingVc = Platform.GetCurrentUIViewController();
        //if (presentingVc == null)
        //{
        //    Log?.LogWarning("UserInteraction.Menu: no window on which to display");
        //    return Task.FromResult(0);
        //}

        //var tcs = new TaskCompletionSource<int>();

        //MainThread.BeginInvokeOnMainThread(() =>
        //{
        //    var currentView = presentingVc.View!;

        //    var alertController = UIAlertController.Create(title, description, UIAlertControllerStyle.ActionSheet);
        //    alertController.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;

        //    if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
        //    {
        //        alertController.ModalPresentationStyle = UIModalPresentationStyle.Popover;
        //        var presenter = alertController.PopoverPresentationController;
        //        presenter.SourceView = currentView;
        //        presenter.SourceRect = position ?? new CGRect(0, currentView.Bounds.Bottom - 1, currentView.Bounds.Width, 1);
        //    }

        //    if (cancelButton != null)
        //        alertController.AddAction(UIAlertAction.Create(cancelButton, UIAlertActionStyle.Cancel, action => tcs.TrySetResult(0)));
        //    if (destroyButton != null)
        //        alertController.AddAction(UIAlertAction.Create(destroyButton, UIAlertActionStyle.Destructive, action => tcs.TrySetResult(1)));

        //    UIAlertAction? defaultAction = null;
        //    var iAction = 2;
        //    foreach (var button in otherButtons)
        //    {
        //        var iActionIndex = iAction++;
        //        if (button != null)
        //        {
        //            var alertAction = UIAlertAction.Create(button, UIAlertActionStyle.Default, action => tcs.TrySetResult(iActionIndex));
        //            alertController.AddAction(alertAction);
        //            if (defaultActionIndex == iActionIndex)
        //                defaultAction = alertAction;
        //        }
        //    }

        //    alertController.PreferredAction = defaultAction;

        //    var registration = dismiss.Register(() => UIApplication.SharedApplication.InvokeOnMainThread(() =>
        //    {
        //        alertController.DismissViewController(true, null);
        //        tcs.TrySetResult(0);
        //    }));

        //    // ReSharper disable once MethodSupportsCancellation
        //    tcs.Task.ContinueWith(t => registration.Dispose());

        //    //Show from bottom
        //    //actionSheet.ShowFrom(new CGRect(0, currentView.Bounds.Bottom - 1, currentView.Bounds.Width, 1), currentView, true);
        //    presentingVc.PresentViewController(alertController, true, null);
        //});

        //return tcs.Task;
    }

    internal static Task PlatformToast(string text, ToastStyle style = ToastStyle.Notice, ToastDuration duration = ToastDuration.Normal, ToastPosition position = ToastPosition.Bottom, int positionOffset = 20, CancellationToken? dismiss = null)
    {
        throw new NotImplementedException();

        //var presentingVc = Platform.GetCurrentUIViewController();
        //if (presentingVc == null)
        //{
        //    Log?.LogWarning("UserInteraction.Toast: no window on which to display");
        //    return Task.CompletedTask;
        //}

        //var tcs = new TaskCompletionSource<int>();

        //MainThread.BeginInvokeOnMainThread(() =>
        //{
        //    var currentView = presentingVc.View!;

        //    //UI items
        //    var font = UIFont.SystemFontOfSize(UIFont.SmallSystemFontSize);
        //    var holder = new UIView {Alpha = 0, BackgroundColor = UIColor.Black };
        //    holder.Layer.CornerRadius = font.LineHeight/2;
        //    holder.Layer.MasksToBounds = true;
        //    var label = new UILabelEx {Text = text, Font = font, TextColor = UIColor.White, TextAlignment = UITextAlignment.Center, LineBreakMode = UILineBreakMode.WordWrap, Lines = 0};

        //    //orders
        //    holder.Add(label);
        //    currentView.Add(holder);
        //    currentView.BringSubviewToFront(holder);

        //    //constraints
        //    holder.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
        //    holder.AddConstraints(
        //        label.AtLeftOf(holder, 10),
        //        label.AtRightOf(holder, 10),
        //        label.AtTopOf(holder, 5),
        //        label.AtBottomOf(holder, 5)
        //    );

        //    holder.TranslatesAutoresizingMaskIntoConstraints = false;
        //    currentView.AddConstraints(
        //        holder.WithSameCenterX(currentView),
        //        holder.Width().LessThanOrEqualTo().WidthOf(currentView).Minus(15*2),
        //        position == ToastPosition.Top ? holder.AtTopOf(currentView, positionOffset) :
        //        position == ToastPosition.Bottom ? holder.AtBottomOf(currentView, positionOffset) :
        //        holder.WithSameCenterY(currentView)
        //    );

        //    //interactions
        //    var inCall = false; //Prevent rebond on tap

        //    void HideHolder(bool animated)
        //    {
        //        if (inCall) return;
        //        inCall = true;

        //        if (animated)
        //        {
        //            UIView.Animate(1f, () => { holder.Alpha = 0; }, () =>
        //            {
        //                holder.RemoveFromSuperview();
        //                holder.Dispose();
        //                tcs.TrySetResult(0);
        //            });
        //        }
        //        else
        //        {
        //            holder.Hidden = true;
        //            holder.RemoveFromSuperview();
        //            holder.Dispose();
        //            tcs.TrySetResult(0);
        //        }
        //    }

        //    CancellationTokenRegistration registration;
        //    if (dismiss.HasValue)
        //    {
        //        registration = dismiss.Value.Register(() => UIApplication.SharedApplication.InvokeOnMainThread(() => HideHolder(false)));
        //        tcs.Task.ContinueWith(t => registration.Dispose());
        //    }
        //    holder.AddGestureRecognizer(new UITapGestureRecognizer(() => HideHolder(true)));

        //    UIView.Animate(1f, () => holder.Alpha = .7f, async () =>
        //    {
        //        await Task.Delay((int)duration, dismiss ?? CancellationToken.None).ConfigureAwait(false);
        //        UIApplication.SharedApplication.InvokeOnMainThread(() => HideHolder(true));
        //    });
        //});

        //return tcs.Task;
    }
}
