using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Text.Method;
using AndroidX.Annotations;
using Google.Android.Material.Dialog;
using Java.Lang;
using Microsoft.Extensions.Logging;
using Exception = System.Exception;
using KeyboardType = Android.Content.Res.KeyboardType;
using String = System.String;
using AndroidX.Core.OS;
using Activity = Android.App.Activity;
using AndroidX.AppCompat.App;
using Google.Android.Material.DatePicker;
using Google.Android.Material.Snackbar;
using Google.Android.Material.TextField;
using ActionMenuView = AndroidX.AppCompat.Widget.ActionMenuView;
using ProgressBar = Android.Widget.ProgressBar;
#if MONOANDROID
using Color = Android.Graphics.Color;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;
using Platform = Xamarin.Essentials.Platform;
#else
using Microsoft.Maui.Platform;
#endif

namespace Vapolia.UserInteraction;


public static class Constants
{
	public static Color?[] ToastStyleBackgroundTint =
	{
		null,
		null,
		null,
#if MONOANDROID
		Color.Orange, 
		Color.Red
#else
		Colors.Orange,
		Colors.Red, 
#endif
	};
}

public partial class UserInteraction
{
	/// <summary>
	/// Not used on Android. Use a theme instead.
	/// </summary>
	internal static uint PlatformDefaultColor { get; set; }
        
	private static Activity? CurrentActivity
	{
		get
		{
			var activity = Platform.CurrentActivity;
			if(activity == null)
				Log?.LogWarning("UserInteraction: Platform.CurrentActivity is null!");
			return activity;
		}
	}

	internal static Task<bool> PlatformConfirm(string message, string? title, string okButton, string cancelButton, CancellationToken? dismiss)
	{
		var tcs = new TaskCompletionSource<bool>();
		var activity = CurrentActivity;
		if (activity != null)
		{
			activity.RunOnUiThread(() =>
			{
				var dialog = new MaterialAlertDialogBuilder(activity)
					.SetMessage(message)!
					.SetTitle(title)!
					.SetCancelable(false)!
					.SetPositiveButton(okButton, (_, _) => tcs.TrySetResult(true))!
					.SetNegativeButton(cancelButton, (_, _) => tcs.TrySetResult(false))!
					.Create()!;

				dialog.Show();

				dismiss?.Register(() =>
				{
					activity.RunOnUiThread(() =>
					{
						dialog.Dismiss();
						tcs.TrySetResult(false);
					});
				});
			});
		}
		else
		{
			tcs.TrySetResult(false);
		}
		return tcs.Task;
	}

	internal static Task<ConfirmThreeButtonsResponse> PlatformConfirmThreeButtons(string message, string? title, string positive, string negative, string neutral)
	{
		var activity = CurrentActivity;
		var tcs = new TaskCompletionSource<ConfirmThreeButtonsResponse>();
		activity?.RunOnUiThread(() =>
		{
			new MaterialAlertDialogBuilder(activity)
				.SetMessage(message)!
				.SetTitle(title)!
				.SetPositiveButton(positive, delegate
				{
					tcs.TrySetResult(ConfirmThreeButtonsResponse.Positive);
				})!
				.SetNegativeButton(negative, delegate
				{
					tcs.TrySetResult(ConfirmThreeButtonsResponse.Negative);
				})!
				.SetNeutralButton(neutral, delegate
				{
					tcs.TrySetResult(ConfirmThreeButtonsResponse.Neutral);
				})!
				.Show();
		});
		return tcs.Task;
	}

	internal static Task PlatformAlert(string message, string title, string okButton)
	{
		var tcs = new TaskCompletionSource<object>();
		var activity = CurrentActivity;
		if (activity != null)
		{
			activity.RunOnUiThread(() =>
			{
				//Crashes on Xamarin Forms Android ?
				new MaterialAlertDialogBuilder(activity)
					.SetMessage(message)
					.SetTitle(title)
					.SetOnCancelListener(new DialogCancelledListener(() => tcs.TrySetResult(false)))
					.SetPositiveButton(okButton, (_,_) => tcs.TrySetResult(true))
					.Show();
			});
		}
		else
		{
			tcs.TrySetResult(false);
		}

		return tcs.Task;
	}

	internal static Task<string?> PlatformInput(string message, string? defaultValue, string? placeholder, string? title, string okButton, string cancelButton, FieldType fieldType, int maxLength, bool selectContent)
	{
		var tcs = new TaskCompletionSource<string?>();

		var activity = CurrentActivity;

		if (activity != null)
		{
			activity.RunOnUiThread(() =>
			{
				var input = new TextInputEditText(activity) {Hint = placeholder, Text = defaultValue };
				if (fieldType == FieldType.Email)
					input.InputType = InputTypes.ClassText | InputTypes.TextVariationEmailAddress;
				else if (fieldType == FieldType.Integer)
					input.InputType = InputTypes.ClassNumber | InputTypes.NumberFlagSigned;
				else if (fieldType == FieldType.Decimal)
				{
					input.InputType = InputTypes.ClassNumber | InputTypes.NumberFlagSigned | InputTypes.NumberFlagDecimal;
					var locale = LocaleListCompat.Default.Get(0);
					try
					{
						input.KeyListener = DigitsKeyListener.GetInstance(locale, true, true);
					}
					catch (NoSuchMethodError)
					{
						if(Build.VERSION.SdkInt >= BuildVersionCodes.O)
							input.KeyListener = new DigitsKeyListener(locale, true, true);
					}
				}

				if (maxLength > 0)
				{
					var filters = input.GetFilters()?.ToList() ?? new List<IInputFilter>();
					filters.Add(new InputFilterLengthFilter(maxLength));
					input.SetFilters(filters.ToArray());
				}

				var layoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
				layoutParameters.SetMargins(DpToPixel(20),0,DpToPixel(20),0);
				input.LayoutParameters = layoutParameters;

				//var layout = new TextInputLayout(activity) { LayoutParameters = layoutParameters };
                //layout.AddView(input);
                var layout = input;

				var dialog = new MaterialAlertDialogBuilder(activity)
					.SetMessage(message)
					.SetTitle(title)
					.SetView(layout)
					.SetCancelable(false)
					.SetPositiveButton(okButton, (_,_) => tcs.TrySetResult(input.Text))
					.SetNegativeButton(cancelButton, (_,_) => tcs.TrySetResult(null))
					.Create();

				if (activity.Resources?.Configuration?.Keyboard == KeyboardType.Nokeys
				    || activity.Resources?.Configuration?.Keyboard == KeyboardType.Undefined
				    || activity.Resources?.Configuration?.HardKeyboardHidden == HardKeyboardHidden.Yes)
				{
					//Show keyboard when input has focus
					input.FocusChange += (sender, args) =>
					{
						if (args.HasFocus)
							dialog.Window?.SetSoftInputMode(SoftInput.StateVisible);
					};
				}

				if (selectContent && !String.IsNullOrWhiteSpace(defaultValue))
					input.RequestFocus();

				dialog.Show();

			});
		}
		else
		{
			tcs.TrySetCanceled();
		}

		return tcs.Task;
	}


	class WaitIndicatorImpl : IWaitIndicator
	{
		private string? title, body;
		public Activity? Context { get; set; }

		public AlertDialog? Dialog { get; set; }

		public WaitIndicatorImpl(CancellationToken userDismissedToken)
			=> UserDismissedToken = userDismissedToken;

		public CancellationToken UserDismissedToken { get; }

		public string? Title 
		{ 
			get => title;
			set
			{
				if (Dialog != null)
				{
					Context?.RunOnUiThread(() =>
					{
						try
						{
							Dialog.SetTitle(value);
						}
						catch (ObjectDisposedException)
						{
							Log?.LogError("WaitIndicator: Title can not be set: Dialog is disposed");
						}
					});
				}

				title = value;
			} 
		}

		public string? Body
		{
			get => body;
			set
			{
				if (Dialog != null)
				{
					Context?.RunOnUiThread(() =>
					{
						try
						{
							Dialog.SetMessage(value);
						}
						catch (ObjectDisposedException)
						{
							Log?.LogError("WaitIndicator: Body can not be set: Dialog is disposed");
						}
					});
				}

				body = value;
			}
		}
	}

	static IWaitIndicator PlatformWaitIndicator(CancellationToken dismiss, string? message, string? title, int? displayAfterSeconds, bool userCanDismiss)
	{
		var cancellationTokenSource = new CancellationTokenSource();
		var wi = new WaitIndicatorImpl(cancellationTokenSource.Token)
		{
			Title = title,
			Body = message
		};
		
		if(displayAfterSeconds is null or 0)
			Do();
		else
		{
			try
			{
				Task.Delay(displayAfterSeconds.Value*1000, dismiss).ContinueWith(_ => Do(), TaskContinuationOptions.OnlyOnRanToCompletion);
			}
			catch (TaskCanceledException)
			{
			}
		}

		return wi;

		void Do()
		{
			var activity = CurrentActivity;

			activity?.RunOnUiThread(() =>
			{
				var input = new ProgressBar(activity, null, Android.Resource.Attribute.ProgressBarStyle)
				{
					Indeterminate = true,
					LayoutParameters = new LinearLayout.LayoutParams(DpToPixel(50), DpToPixel(50)) {Gravity = GravityFlags.CenterHorizontal | GravityFlags.CenterVertical},
				};

				var dialog = new MaterialAlertDialogBuilder(activity)
					.SetTitle(wi.Title)
					?.SetMessage(wi.Body)
					?.SetView(input)
					?.SetCancelable(userCanDismiss)
					?.SetOnCancelListener(new DialogCancelledListener(cancellationTokenSource.Cancel))
					?.Create();

				if (dialog == null)
					throw new("UserInteraction can't create dialog, MaterialAlertDialogBuilder returned null.");

				dialog.SetCanceledOnTouchOutside(userCanDismiss);
				//dialog.CancelEvent += delegate { cancellationTokenSource.Cancel(); };

				wi.Context = activity;
				wi.Dialog = dialog;
				dialog.Show();
				dismiss.Register(() =>
				{
					wi.Dialog = null;
					activity.RunOnUiThread(dialog.Dismiss);
				});
			});
		}
	}

	class DialogDismissListener : Java.Lang.Object, IDialogInterfaceOnDismissListener
	{
		private readonly Func<bool> onDismiss;

		public DialogDismissListener(Func<bool> onDismiss)
			=> this.onDismiss = onDismiss;

		public void OnDismiss(IDialogInterface? dialog)
			=> onDismiss();
	}

	static Task PlatformActivityIndicator(CancellationToken dismiss, double? apparitionDelay, uint? argbColor)
	{
		var tcs = new TaskCompletionSource<int>();

		Task.Delay((int)((apparitionDelay ?? 0)*1000+.5), dismiss).ContinueWith(t =>
		{
			var activity = CurrentActivity;
			if (t.Status == TaskStatus.RanToCompletion && activity != null)
			{
				activity.RunOnUiThread(() =>
				{
					var layout = new FrameLayout(activity) {LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,ViewGroup.LayoutParams.MatchParent) };
					var input = new ProgressBar(activity) { Indeterminate = true, LayoutParameters = new FrameLayout.LayoutParams(DpToPixel(100), DpToPixel(100)) {Gravity = GravityFlags.Center}};
					layout.AddView(input);

					var builder = new MaterialAlertDialogBuilder(activity);
					builder
						.SetView(layout)
						.SetCancelable(false)
						.SetOnDismissListener(new DialogDismissListener(() => tcs.TrySetResult(0)));

					//Make translucent. ThemeTranslucentNoTitleBarFullScreen does not work on wiko.
					builder.SetBackground(new ColorDrawable(Android.Graphics.Color.Argb(175,255,255,255))); 
					var dialog = builder.Show();

					dismiss.Register(() =>
					{
						activity.RunOnUiThread(() =>
						{
							if (dialog.IsShowing)
							{
								try
								{
									dialog.Dismiss();
								}
								catch (Exception)
								{
									//Dialog dismissed
									Log?.LogError("Exception while dismissing dialog, is the activity hidden before the dialog has been dismissed ?");
								}
							}
						});
						tcs.TrySetResult(0);
					});
				});
			}
			else
				tcs.TrySetResult(0);
		});

		return tcs.Task;	   
	}

	/// <summary>
	/// Displays a system menu.
	/// If otherButtons is null, the indexes are still incremented, but the button won't appear. 
	/// This enables easy scenario where the otherButtons array is changing between calls.
	/// </summary>
	/// <param name="dismiss"></param>
	/// <param name="userCanDismiss">true if the user can cancel the menu using the android gesture or button</param>
	/// <param name="position">optional: position from top left of screen</param>
	/// <param name="title"></param>
	/// <param name="description">NOT SUPPORTED ON ANDROID (setting it hides the items by design with the default template)</param>
	/// <param name="defaultActionIndex">from 2 to 2+number of actions. Otherwise ignored.</param>
	/// <param name="cancelButton">NOT DISPLAYED ON ANDROID (use the standard dismiss gesture instead)</param>
	/// <param name="destroyButton">optional</param>
	/// <param name="otherButtons">If a button is null, the indexes are still incremented, but the button won't appear</param>
	/// <returns>
	/// Button indexes:
	/// cancel: 0 (never displayed on Android. Use hardware back button instead)
	/// destroy: 1
	/// others: 2+index
	/// </returns>
	internal static Task<int> PlatformMenu(CancellationToken dismiss, bool userCanDismiss,
		System.Drawing.RectangleF? position = null,
		string? title = null, string? description = null, int defaultActionIndex = -1, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons)
	{
		var tcs = new TaskCompletionSource<int>();

		void CancelAction() => tcs.TrySetResult(0);

		var activity = CurrentActivity;
		if (activity != null)
		{
			activity.RunOnUiThread(() =>
			{
				var cancelButtonIndex = -1;
				var destructiveButtonIndex = -1;

				var items = new List<string>();
				if (destroyButton != null)
				{
					items.Add(destroyButton);
					destructiveButtonIndex = 0;
				}
				if(otherButtons != null)
					items.AddRange(otherButtons);
				//if (cancelButton != null)
				//{
				//items.Add(cancelButton);
				//cancelButtonIndex = items.Count - 1;
				//}

				AlertDialog ad = null!;

				void OnItemSelected(DialogClickEventArgs args, bool closeOnSelect)
				{
					var buttonIndex = args.Which;

					if ((cancelButton != null && buttonIndex == cancelButtonIndex) || buttonIndex < 0)
						tcs.TrySetResult(0);
					else if (destroyButton != null && buttonIndex == destructiveButtonIndex)
						tcs.TrySetResult(1);
					else
					{
						if (destructiveButtonIndex >= 0)
							buttonIndex++;
						else
							buttonIndex += 2;

						//Correct the index given the number of holes
						var n = buttonIndex - 2;
						var realIndex = 0;
						while (n != 0)
						{
							if (items[realIndex++] != null) n--;
						}

						tcs.TrySetResult(2 + realIndex);
					}

					if(closeOnSelect)
						ad.Dismiss();
				}

				var adBuilder = (MaterialAlertDialogBuilder)new MaterialAlertDialogBuilder(activity)
					.SetTitle(title) //Titles on AlertDialogs are limited to 2 lines, and if SetMessage is used SetItems does not work.
					.SetCancelable(userCanDismiss);

				if (defaultActionIndex < 2)
					adBuilder.SetItems(items.Where(b => b != null).ToArray(), (sender, args) => OnItemSelected(args, false));
				else
					adBuilder.SetSingleChoiceItems(items.Where(b => b != null).ToArray(), defaultActionIndex - 2 + (destructiveButtonIndex >= 0 ? 1 : 0), (sender, args) => OnItemSelected(args, true));

				//Setting a description hide the items. This is by design in the dialog template.
				//https://stackoverflow.com/questions/10714911/alertdialogs-items-not-displayed
				//if (description != null)
				//    adBuilder.SetMessage(description);

				ad = adBuilder.Create();
				ad.SetCanceledOnTouchOutside(userCanDismiss);
				ad.CancelEvent += (sender, args) => CancelAction();
				ad.DismissEvent += (sender, args) => CancelAction();

				if (position != null)
				{
					//ad.RequestWindowFeature((int)WindowFeatures.NoTitle);
					var layoutParams = ad.Window?.Attributes;
					if (layoutParams != null)
					{
						layoutParams.Gravity = GravityFlags.Top | GravityFlags.Left;
						layoutParams.X = DpToPixel(position.Value.Left);
						layoutParams.Y = DpToPixel(position.Value.Top);
						ad.Window!.Attributes = layoutParams;
					}
				}

				ad.Show();

				dismiss.Register(() =>
				{
					activity.RunOnUiThread(() =>
					{
						ad.Dismiss();
						CancelAction();
					});
				});
			});
		}
		else
			tcs.TrySetResult(0);

		return tcs.Task;	   
	}

	internal static Task PlatformToast(string text, ToastStyle style = ToastStyle.Notice, ToastDuration duration = ToastDuration.Normal, ToastPosition position = ToastPosition.Bottom, int positionOffset = 20, CancellationToken? dismiss = null)
	{
		var tcs = new TaskCompletionSource<int>();

		var activity = CurrentActivity;
		var view = activity?.Window?.DecorView.RootView;
		if (activity != null && view != null)
		{
			activity.RunOnUiThread(() =>
			{
				var snackBar = Snackbar.Make(activity, view, text, duration == ToastDuration.Short ? BaseTransientBottomBar.LengthShort : BaseTransientBottomBar.LengthLong);
				//var toast = Android.Widget.Toast.MakeText(activity, text, duration == ToastDuration.Short ? ToastLength.Short : ToastLength.Long);
				if(snackBar != null)
				{
					var color = Constants.ToastStyleBackgroundTint[(int)style];
                    if (color != null)
                    {
						#if MONOANDROID
                        var platformColor = color.Value;
                        #else
						var platformColor = color.ToPlatform();
						#endif
                        snackBar.SetBackgroundTint(platformColor);
                    }

                    var layoutParams = snackBar.View.LayoutParameters as FrameLayout.LayoutParams;
					if (layoutParams != null)
					{
						layoutParams.Gravity = position == ToastPosition.Bottom ? GravityFlags.Bottom : (position == ToastPosition.Top ? GravityFlags.Top : GravityFlags.CenterVertical);
						layoutParams.SetMargins(0, position == ToastPosition.Top ? positionOffset : 0, 0, position == ToastPosition.Bottom ? positionOffset : 0);
						snackBar.View.LayoutParameters = layoutParams;
					}

					dismiss?.Register(() => activity.RunOnUiThread(() => snackBar.Dismiss()));
					snackBar.Show();
				}

				tcs.TrySetResult(0);
			});
		}
		else
			tcs.TrySetResult(0);

		return tcs.Task;
	}

	private static int DpToPixel(float dp) => (int)(dp*((int)(CurrentActivity?.Resources!.DisplayMetrics!.DensityDpi ?? 0))/160f+.5);
}

internal class DialogCancelledListener : Java.Lang.Object, IDialogInterfaceOnCancelListener
{
	private readonly Action action;
	public DialogCancelledListener(Action action) => this.action = action;
	public void OnCancel(IDialogInterface? dialog) => action?.Invoke();
}