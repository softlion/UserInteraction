using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.App;
using Android.Runtime;
using Microsoft.Extensions.Logging;
using KeyboardType = Android.Content.Res.KeyboardType;

namespace Vapolia.UserInteraction.Droid
{
    /// <summary>
    /// Theming
    ///
    ///<![CDATA[
    ///<style name="MyAlertDialogStyle" parent="Theme.AppCompat.Light.Dialog.Alert">
    ///   <!-- Used for the buttons -->
    ///   <item name="colorAccent">#FFC107</item>
    ///   <!-- Used for the title and text -->
    ///   <item name="android:textColorPrimary">#FFFFFF</item>
    ///   <!-- Used for the background -->
    ///   <item name="android:background">#4CAF50</item>
    ///</style>
    ///
    ///In order to change the Appearance of the Title, you can do the following. First add a new style:
    ///
    ///<style name="MyTitleTextStyle">
    ///   <item name="android:textColor">#FFEB3B</item>
    ///   <item name="android:textAppearance">@style/TextAppearance.AppCompat.Title</item>
    ///</style>
    ///afterwards simply reference this style in your MyAlertDialogStyle:
    ///
    ///<style name="MyAlertDialogStyle" parent="Theme.AppCompat.Light.Dialog.Alert">
    ///   ...
    ///   <item name="android:windowTitleStyle">@style/MyTitleTextStyle</item>
    ///</style>    
    ///]]>
    /// </summary>
	public sealed class UserInteraction : IUserInteraction
	{
        private readonly ILogger log;
        private int themeResId;
        private Activity? CurrentActivity
        {
            get
            {
                var activity = Xamarin.Essentials.Platform.CurrentActivity;
                if(activity == null)
                    log.LogWarning("UserInteraction: IMvxAndroidCurrentTopActivity.Activity is null!");
                return activity;
            }
        }

        public int ThemeResId
        {
            get => themeResId;
            set => themeResId = value;
        }

        /// <summary>
        /// Not used. In android use global styles instead.
        /// </summary>
        public uint DefaultColor { set {} }


        //public void Confirm(string message, Action okClicked, string title = null, string okButton = "OK", string cancelButton = "Cancel")
        //{
        //    Confirm(message, confirmed => {
        //        if (confirmed)
        //            okClicked();
        //    },
        //    title, okButton, cancelButton);
        //}

        //public void Confirm(string message, Action<bool> answer, string title = null, string okButton = "OK", string cancelButton = "Cancel")
        //{
        //    //Mvx.Resolve<IMvxMainThreadDispatcher>().RequestMainThreadAction();
        //    if (CurrentActivity != null)
        //    {
        //        CurrentActivity.RunOnUiThread(() => 
        //            new AlertDialog.Builder(CurrentActivity,themeResId)
        //            .SetMessage(message)
        //            .SetTitle(title)
        //            .SetPositiveButton(okButton, delegate
        //            {
        //                if (answer != null)
        //                    answer(true);
        //            })
        //            .SetNegativeButton(cancelButton, delegate
        //            {
        //                if (answer != null)
        //                    answer(false);
        //            })
        //            .Show());
        //    }
        //}

        [Preserve(Conditional = true)]
        public UserInteraction(ILogger logger)
        {
            log = logger;
        }

        [Preserve(Conditional = true)]
        public UserInteraction(ILogger logger, int themeResId)
        {
            log = logger;
            this.themeResId = themeResId;
        }

        public Task<bool> Confirm(string message, string? title = null, string okButton = "OK", string cancelButton = "Cancel", CancellationToken? dismiss = null)
		{
		    var tcs = new TaskCompletionSource<bool>();
		    var activity = CurrentActivity;
            if (activity != null)
		    {
                activity.RunOnUiThread(() =>
                {
                    var dialog = new AlertDialog.Builder(activity,themeResId)!
                        .SetMessage(message)!
                        .SetTitle(title)!
                        .SetCancelable(false)!
                        .SetPositiveButton(okButton, (sender, args) => 
                        {
                            tcs.TrySetResult(true);
                        })!
                        .SetNegativeButton(cancelButton, (sender, args) => 
                        {
                            tcs.TrySetResult(false);
                        })!
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

	    public void ConfirmThreeButtons(string message, Action<ConfirmThreeButtonsResponse> answer, string? title = null, string positive = "Yes", string negative = "No",
	        string neutral = "Maybe")
	    {
	        var activity = CurrentActivity;
	        activity?.RunOnUiThread(() =>
	        {
	            new AlertDialog.Builder(activity,themeResId)!
	                .SetMessage(message)!
	                .SetTitle(title)!
	                .SetPositiveButton(positive, delegate
	                {
                        answer.Invoke(ConfirmThreeButtonsResponse.Positive);
                    })!
	                .SetNegativeButton(negative, delegate
	                {
                        answer.Invoke(ConfirmThreeButtonsResponse.Negative);
                    })!
	                .SetNeutralButton(neutral, delegate
	                {
                        answer.Invoke(ConfirmThreeButtonsResponse.Neutral);
                    })!
	                .Show();
	        });
	    }

        //public void Alert(string message, Action done = null, string title = "", string okButton = "OK")
        //{
        //    if (CurrentActivity != null)
        //    {
        //        CurrentActivity.RunOnUiThread(() =>
        //        {
        //            new AlertDialog.Builder(CurrentActivity,themeResId)
        //                .SetMessage(message)
        //                .SetTitle(title)
        //                .SetPositiveButton(okButton, (s,e) =>
        //                {
        //                    if (done != null)
        //                        done();
        //                })
        //                .Show();
        //        });
        //    }
        //}


		public Task Alert(string message, string title = "", string okButton = "OK")
		{
            var tcs = new TaskCompletionSource<object>();
		    var activity = CurrentActivity;
            if (activity != null)
            {
                activity.RunOnUiThread(() =>
                {
                    var dialog = new AlertDialog.Builder(activity,themeResId)!
                        .SetMessage(message)!
                        .SetTitle(title)!
                        .SetOnCancelListener(new DialogCancelledListener(() =>
                        {
                            tcs.TrySetResult(false);
                        }))!
                        .SetPositiveButton(okButton, (s,e) =>
                        {
                            tcs.TrySetResult(true);
                        })!;
                    dialog.Show();
                });
            }
            else
            {
                tcs.TrySetResult(false);
            }

		    return tcs.Task;
		}

	    public Task<string?> Input(string message, string? defaultValue = null, string? placeholder = null, string? title = null, string okButton = "OK", string cancelButton = "Cancel", FieldType fieldType = FieldType.Default, int maxLength=0)
	    {
	        var tcs = new TaskCompletionSource<string?>();

	        var activity = CurrentActivity;

            if (activity != null)
	        {
                activity.RunOnUiThread(() =>
                {
		            var input = new EditText(activity) {Hint = placeholder, Text = defaultValue };
	                if (fieldType == FieldType.Email)
	                    input.InputType = InputTypes.ClassText | InputTypes.TextVariationEmailAddress;
			        else if (fieldType == FieldType.Integer)
                        input.InputType = InputTypes.ClassNumber; // | InputTypes.NumberFlagDecimal;

	                if (maxLength > 0)
	                {
	                    var filters = input.GetFilters().ToList();
	                    filters.Add(new InputFilterLengthFilter(maxLength));
	                    input.SetFilters(filters.ToArray());
	                }

	                var dialog = new AlertDialog.Builder(activity,themeResId)!
		                .SetMessage(message)!
		                .SetTitle(title)!
		                .SetView(input)!
	                    .SetCancelable(false)!
		                .SetPositiveButton(okButton, delegate
		                {
		                    tcs.TrySetResult(input.Text);
		                })!
		                .SetNegativeButton(cancelButton, delegate
		                {
		                    tcs.TrySetResult(null);
		                })!
                        .Create()!;

	                if (activity.Resources.Configuration.Keyboard == KeyboardType.Nokeys
	                    || activity.Resources.Configuration.Keyboard == KeyboardType.Undefined
	                    || activity.Resources.Configuration.HardKeyboardHidden == HardKeyboardHidden.Yes)
	                {
	                    //Show keyboard when input has focus
	                    input.FocusChange += (sender, args) =>
	                    {
	                        if (args.HasFocus)
	                            dialog.Window.SetSoftInputMode(SoftInput.StateVisible);
	                    };
	                }

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
	        private string title, body;
	        public Activity Context { get; set; }

            public AlertDialog Dialog { get; set; }

            public WaitIndicatorImpl(CancellationToken userDismissedToken)
            {
                UserDismissedToken = userDismissedToken;
            }

            public CancellationToken UserDismissedToken { get; }
	        public string Title { set { if(Dialog!=null) Context?.RunOnUiThread(() =>
	        {
	            try
	            {
	                Dialog.SetTitle(value);
	            }
	            catch(ObjectDisposedException)
	            {
	                Console.WriteLine("WaitIndicator: Title can not be set: Dialog is disposed");
	            }
	        }); title = value; } get => title; }
	        public string Body { set { if (Dialog != null) Context?.RunOnUiThread(() =>
	        {
	            try
	            {
	                Dialog.SetMessage(value);
	            }
	            catch(ObjectDisposedException)
	            {
	                Console.WriteLine("WaitIndicator: Body can not be set: Dialog is disposed");
	            }
	        }); body = value; } get => body; }
	    }

	    public IWaitIndicator WaitIndicator(CancellationToken dismiss, string message = null, string title = null, int? displayAfterSeconds = null, bool userCanDismiss = true)
	    {
            var cancellationTokenSource = new CancellationTokenSource();
	        var wi = new WaitIndicatorImpl(cancellationTokenSource.Token)
	        {
                Title = title,
                Body = message
	        };

            Task.Delay((displayAfterSeconds ?? 0)*1000, dismiss).ContinueWith(t =>
	        {
	            var activity = CurrentActivity;

	            activity?.RunOnUiThread(() =>
	            {
	                var input = new ProgressBar(activity, null, Android.Resource.Attribute.ProgressBarStyle)
	                {
	                    Indeterminate = true,
	                    LayoutParameters = new LinearLayout.LayoutParams(DpToPixel(50), DpToPixel(50)) {Gravity = GravityFlags.CenterHorizontal | GravityFlags.CenterVertical},
	                };

	                var dialog = new AlertDialog.Builder(activity,themeResId)
	                    .SetTitle(wi.Title)
	                    .SetMessage(wi.Body)
	                    .SetView(input)
	                    .SetCancelable(userCanDismiss)
	                    .SetOnCancelListener(new DialogCancelledListener(cancellationTokenSource.Cancel))
	                    .Create();
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
	        }, TaskContinuationOptions.OnlyOnRanToCompletion);

	        return wi;
        }

        public Task ActivityIndicator(CancellationToken dismiss, double? apparitionDelay = null, uint? argbColor = null)
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

                        /*var dialog = new AlertDialog.Builder(activity,themeResId)
                            .SetView(input)
                            .SetCancelable(false)
                            .Create();*/

                        var dialog = new AndroidX.AppCompat.App.AppCompatDialog(activity, Android.Resource.Style.ThemeNoTitleBar); //Theme_Translucent //ThemeNoTitleBarFullScreen
                        dialog.SetContentView(layout);
                        dialog.SetCancelable(false);
                        //dialog.CancelEvent += (sender, args) => tcs.TrySetResult(0);
                        dialog.DismissEvent += (sender, args) => tcs.TrySetResult(0);

                        //Make translucent. ThemeTranslucentNoTitleBarFullScreen does not work on wiko.
                        dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Argb(175,255,255,255))); 
                        dialog.Show();

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
                                        //Mvx.Trace("Exception while dismissing dialog, is the activity hidden before the dialog has been dismissed ?");
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

        public Task<int> Menu(CancellationToken dismiss, bool userCanDismiss, string title, string cancelButton, string destroyButton, params string[] otherButtons)
        {
            return Menu(dismiss, userCanDismiss, title, null, -1, cancelButton, destroyButton, otherButtons);
        }

        /// <summary>
        /// Displays a system menu.
        /// If otherButtons is null, the indexes are still incremented, but the button won't appear. 
        /// This enables easy scenario where the otherButtons array is changing between calls.
        /// </summary>
        /// <param name="dismiss"></param>
        /// <param name="userCanDismiss"></param>
        /// <param name="title"></param>
        /// <param name="description">optional</param>
        /// <param name="defaultActionIndex">from 2 to 2+number of actions. Otherwise ignored.</param>
        /// <param name="cancelButton">optional</param>
        /// <param name="destroyButton">optional</param>
        /// <param name="otherButtons">If a button is null, the index are still incremented, but the button won't appear</param>
        /// <returns>
        /// Button indexes:
        /// cancel: 0 (never displayed on Android. Use hardware back button instead)
        /// destroy: 1
        /// others: 2+index
        /// </returns>
        public Task<int> Menu(CancellationToken dismiss, bool userCanDismiss, string title, string description, int defaultActionIndex, string cancelButton, string destroyButton, params string[] otherButtons)
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

                    AlertDialog ad = null;

                    void OnItemSelected(DialogClickEventArgs args, bool closeOnSelect)
                    {
                        var buttonIndex = args.Which;
                        //Mvx.Trace("Dialog item clicked: {0}", buttonIndex);

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

                    var adBuilder = new AlertDialog.Builder(activity, themeResId)
                        .SetTitle(title) //Titles on AlertDialogs are limited to 2 lines, and if SetMessage is used SetItems does not work.
                        //.SetInverseBackgroundForced(true) //This method was deprecated in API level 23. This flag is only used for pre-Material themes. Instead, specify the window background using on the alert dialog theme.
                        .SetCancelable(userCanDismiss);

                    if (defaultActionIndex < 2)
                        adBuilder.SetItems(items.Where(b => b != null).ToArray(), (sender, args) => OnItemSelected(args, false));
                    else
                        adBuilder.SetSingleChoiceItems(items.Where(b => b != null).ToArray(), defaultActionIndex - 2 + (destructiveButtonIndex >= 0 ? 1 : 0), (sender, args) => OnItemSelected(args, true));


                    if (description != null)
                        adBuilder.SetMessage(description);

                    ad = adBuilder.Create();
                    ad.SetCanceledOnTouchOutside(userCanDismiss);

	                ad.CancelEvent += (sender, args) =>
	                {
                        //Mvx.Trace("Dialog cancelled");
	                    CancelAction();
	                };
	                ad.DismissEvent += (sender, args) =>
	                {
                        //Mvx.Trace("Dialog dismissed");
	                    CancelAction();
	                };
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
	        {
	            tcs.TrySetResult(0);
	        }

	        return tcs.Task;	   
        }

        public Task Toast(string text, ToastStyle style = ToastStyle.Notice, ToastDuration duration = ToastDuration.Normal, ToastPosition position = ToastPosition.Bottom, int positionOffset = 20, CancellationToken? dismiss = null)
        {
            var tcs = new TaskCompletionSource<int>();

            var activity = CurrentActivity;
            if (activity != null)
            {
                activity.RunOnUiThread(() =>
                {
                    var toast = Android.Widget.Toast.MakeText(activity, text, duration == ToastDuration.Short ? ToastLength.Short : ToastLength.Long);
                    toast.SetGravity((position == ToastPosition.Bottom ? GravityFlags.Bottom : (position == ToastPosition.Top ? GravityFlags.Top : GravityFlags.CenterVertical))|GravityFlags.CenterHorizontal, 0, positionOffset);

                    dismiss?.Register(() => activity.RunOnUiThread(() => toast.Cancel()));
                    toast.Show();

                    tcs.TrySetResult(0);
                });
            }
	        else
	        {
	            tcs.TrySetResult(0);
	        }

            return tcs.Task;	   
        }

        private static int DpToPixel(float dp)
	    {
	        return (int)(dp*((int)Application.Context.Resources.DisplayMetrics.DensityDpi)/160f+.5);
	    }
	}

    internal class DialogCancelledListener : Java.Lang.Object, IDialogInterfaceOnCancelListener
    {
        private readonly Action action;

        public DialogCancelledListener(Action action)
        {
            this.action = action;
        }

        public void OnCancel(IDialogInterface dialog)
        {
            action?.Invoke();
        }
    }
}

