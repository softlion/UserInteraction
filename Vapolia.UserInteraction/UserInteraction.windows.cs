using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Maui.Platform;
using Window = Microsoft.UI.Xaml.Window;

namespace Vapolia.UserInteraction;

public partial class UserInteraction
{
    private static Microsoft.UI.Xaml.Media.Brush? PlatformDefaultColor => DefaultColor?.ToPlatform();
    
    private static Window? CurrentWindow
    {
        get
        {
            var window = GetWindow().Handler?.PlatformView as Window;
            if(window == null)
                Log?.LogWarning("UserInteraction: can't get current window, it's null.");
            return window;
        }
    }

    internal static Task<bool> PlatformConfirm(string message, string? title = null, string okButton = "OK", string cancelButton = "Cancel", CancellationToken? dismiss = null)
    {
        var tcs = new TaskCompletionSource<bool>();
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var currentWindow = CurrentWindow;
            if (currentWindow == null)
            {
                tcs.TrySetResult(false);
                return;
            }

            var confirm = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = cancelButton,
                PrimaryButtonText = okButton,
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = currentWindow.Content.XamlRoot
            };

            CancellationTokenRegistration? registration = null;
            if (dismiss != null)
            {
                registration = dismiss.Value.Register(() =>
                {
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        try
                        {
                            confirm.Hide();
                        }
                        catch
                        {
                            // Dialog might already be closed
                        }
                        tcs.TrySetResult(false);
                    });
                });
            }

            try
            {
                var result = await confirm.ShowAsync();
                tcs.TrySetResult(result == ContentDialogResult.Primary);
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, "Error showing confirm dialog");
                tcs.TrySetResult(false);
            }
            finally
            {
                registration?.Dispose();
            }
        });

        return tcs.Task;
    }

    internal static Task<ConfirmThreeButtonsResponse> PlatformConfirmThreeButtons(string message, string? title = null, string positive = "Yes", string negative = "No", string neutral = "Maybe")
    {
        var tcs = new TaskCompletionSource<ConfirmThreeButtonsResponse>();
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var currentWindow = CurrentWindow;
            if (currentWindow == null)
            {
                tcs.TrySetResult(ConfirmThreeButtonsResponse.Negative);
                return;
            }

            var confirm = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = positive,
                SecondaryButtonText = neutral,
                CloseButtonText = negative,
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = currentWindow.Content.XamlRoot
            };

            try
            {
                var result = await confirm.ShowAsync();
                switch (result)
                {
                    case ContentDialogResult.Primary:
                        tcs.TrySetResult(ConfirmThreeButtonsResponse.Positive);
                        break;
                    case ContentDialogResult.Secondary:
                        tcs.TrySetResult(ConfirmThreeButtonsResponse.Neutral);
                        break;
                    default:
                        tcs.TrySetResult(ConfirmThreeButtonsResponse.Negative);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, "Error showing three buttons confirm dialog");
                tcs.TrySetResult(ConfirmThreeButtonsResponse.Negative);
            }
        });

        return tcs.Task;
    }

    /// <summary>
    /// Shows an alert dialog with a message and a single button.
    /// </summary>
    /// <remarks>
    /// Note: In WinUI 3, Microsoft.UI.Xaml.Application.Current.Windows collection doesn't exist anymore.
    /// This is a breaking change from UWP. The proper way to get the current window in WinUI 3 is to:
    /// 1. Use the Window.AppWindow property (Windows App SDK 1.3+)
    /// 2. Or use WindowNative.GetWindowHandle and AppWindow.GetFromWindowId
    /// 3. Or implement a WindowHelper class that tracks all windows in your application
    ///
    /// For a complete implementation, you should maintain a reference to your main window
    /// or implement a WindowHelper class in your application.
    /// </remarks>
    internal static Task PlatformAlert(string message, string title = "", string okButton = "OK")
    {
        var tcs = new TaskCompletionSource();
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var currentWindow = CurrentWindow;
            if (currentWindow == null)
            {
                tcs.TrySetResult();
                return;
            }

            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = okButton,
                XamlRoot = currentWindow.Content.XamlRoot,
                DefaultButton = ContentDialogButton.Close
            };

            await dialog.ShowAsync();
            tcs.TrySetResult();
        });
        
        return tcs.Task;
    }

    

    internal static Task<string?> PlatformInput(string message, string? defaultValue = null, string? placeholder = null, string? title = null, string okButton = "OK", string cancelButton = "Cancel", FieldType fieldType = FieldType.Default, int maxLength = 0, bool selectContent = true)
    {
        var tcs = new TaskCompletionSource<string?>();
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var currentWindow = CurrentWindow;
            if (currentWindow == null)
            {
                tcs.TrySetResult(null);
                return;
            }

            var textBox = new Microsoft.UI.Xaml.Controls.TextBox
            {
                Text = defaultValue ?? string.Empty,
                PlaceholderText = placeholder ?? string.Empty,
                MaxLength = maxLength > 0 ? maxLength : 0
            };

            // Configure input scope based on field type
            var inputScope = new Microsoft.UI.Xaml.Input.InputScope();
            var inputScopeName = new Microsoft.UI.Xaml.Input.InputScopeName();

            switch (fieldType)
            {
                case FieldType.Email:
                    inputScopeName.NameValue = Microsoft.UI.Xaml.Input.InputScopeNameValue.EmailSmtpAddress;
                    break;
                case FieldType.Integer:
                    inputScopeName.NameValue = Microsoft.UI.Xaml.Input.InputScopeNameValue.Number;
                    break;
                case FieldType.Decimal:
                    inputScopeName.NameValue = Microsoft.UI.Xaml.Input.InputScopeNameValue.Number;
                    break;
                default:
                    inputScopeName.NameValue = Microsoft.UI.Xaml.Input.InputScopeNameValue.Default;
                    break;
            }

            inputScope.Names.Add(inputScopeName);
            textBox.InputScope = inputScope;

            if (selectContent && !string.IsNullOrWhiteSpace(defaultValue))
            {
                textBox.Loaded += (s, e) =>
                {
                    textBox.SelectAll();
                    textBox.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
                };
            }

            var dialog = new ContentDialog
            {
                Title = title,
                Content = new Microsoft.UI.Xaml.Controls.StackPanel
                {
                    Children =
                    {
                        new Microsoft.UI.Xaml.Controls.TextBlock { Text = message, Margin = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 10) },
                        textBox
                    }
                },
                PrimaryButtonText = okButton,
                CloseButtonText = cancelButton,
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = currentWindow.Content.XamlRoot
            };

            try
            {
                var result = await dialog.ShowAsync();
                tcs.TrySetResult(result == ContentDialogResult.Primary ? textBox.Text : null);
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, "Error showing input dialog");
                tcs.TrySetResult(null);
            }
        });

        return tcs.Task;
    }

    class WaitIndicatorImpl(CancellationToken userDismissedToken) : IWaitIndicator
    {
        private string? title, body;

        public ContentDialog? Dialog { get; set; }

        public CancellationToken UserDismissedToken { get; } = userDismissedToken;

        public string? Title
        {
            get => title;
            set
            {
                title = value;
                if (Dialog != null)
                    MainThread.InvokeOnMainThreadAsync(() => Dialog.Title = value);
            }
        }

        public string? Body
        {
            get => body;
            set
            {
                body = value;
                if (Dialog != null)
                {
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        if (Dialog.Content is Microsoft.UI.Xaml.Controls.StackPanel panel &&
                            panel.Children.Count > 1 &&
                            panel.Children[1] is Microsoft.UI.Xaml.Controls.TextBlock textBlock)
                        {
                            textBlock.Text = value ?? string.Empty;
                        }
                    });
                }
            }
        }
    }

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
        var userDismissed = new CancellationTokenSource();
        var wi = new WaitIndicatorImpl(userDismissed.Token)
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
            catch (Exception e) when (e is TaskCanceledException or OperationCanceledException)
            {
                //
            }
        }

        return wi;

        void Do()
        {
            MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var currentWindow = CurrentWindow;
                if (currentWindow == null)
                {
                    Log?.LogWarning("WaitIndicator: no window on which to display");
                    return;
                }

                var progressRing = new Microsoft.UI.Xaml.Controls.ProgressRing
                {
                    IsActive = true,
                    Width = 50,
                    Height = 50,
                    Margin = new Microsoft.UI.Xaml.Thickness(0, 10, 0, 10)
                };

                var messageTextBlock = new Microsoft.UI.Xaml.Controls.TextBlock
                {
                    Text = message ?? string.Empty,
                    TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
                    HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center
                };

                var content = new Microsoft.UI.Xaml.Controls.StackPanel
                {
                    HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                    Children = { progressRing, messageTextBlock }
                };

                var dialog = new ContentDialog
                {
                    Title = title ?? string.Empty,
                    Content = content,
                    XamlRoot = currentWindow.Content.XamlRoot
                };

                if (userCanDismiss)
                {
                    dialog.CloseButtonText = "Cancel";
                    dialog.CloseButtonClick += (s, e) => userDismissed.Cancel();
                }

                wi.Dialog = dialog;

                var registration = dismiss.Register(() =>
                {
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        try
                        {
                            dialog.Hide();
                        }
                        catch
                        {
                            // Dialog might already be closed
                        }
                    });
                });

                try
                {
                    await dialog.ShowAsync();
                }
                catch (Exception ex)
                {
                    Log?.LogError(ex, "Error showing wait indicator");
                }
                finally
                {
                    await registration.DisposeAsync();
                }
            });
        }
    }

    static Task PlatformActivityIndicator(CancellationToken dismiss, double? apparitionDelay = null, uint? argbColor = null)
    {
        var currentWindow = CurrentWindow;
        if (currentWindow == null)
        {
            Log?.LogWarning("UserInteraction.ActivityIndicator: no window on which to display");
            return Task.CompletedTask;
        }

        var tcs = new TaskCompletionSource<int>();

        Task.Delay((int)((apparitionDelay ?? 0)*1000+.5), dismiss).ContinueWith(t =>
        {
            if (!t.IsCanceled)
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    var currentContent = currentWindow.Content;
                    if (currentContent == null)
                    {
                        tcs.TrySetResult(0);
                        return;
                    }

                    // Create overlay
                    var overlay = new Microsoft.UI.Xaml.Controls.Grid
                    {
                        Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black) { Opacity = 0.5 },
                        HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                        VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch
                    };

                    // Create progress ring
                    var progressRing = new Microsoft.UI.Xaml.Controls.ProgressRing
                    {
                        IsActive = true,
                        Width = 60,
                        Height = 60,
                        HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                        VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center
                    };

                    // Apply color if specified
                    if (argbColor.HasValue)
                    {
                        var color = Microsoft.UI.ColorHelper.FromArgb(
                            (byte)((argbColor.Value >> 24) & 0xFF),
                            (byte)((argbColor.Value >> 16) & 0xFF),
                            (byte)((argbColor.Value >> 8) & 0xFF),
                            (byte)(argbColor.Value & 0xFF));
                        progressRing.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(color);
                    }
                    else if (PlatformDefaultColor != null)
                    {
                        progressRing.Foreground = PlatformDefaultColor;
                    }

                    overlay.Children.Add(progressRing);

                    // Add to current content if it's a Panel
                    if (currentContent is Microsoft.UI.Xaml.Controls.Panel panel)
                    {
                        panel.Children.Add(overlay);
                    }
                    else if (currentContent is Microsoft.UI.Xaml.Controls.Grid grid)
                    {
                        grid.Children.Add(overlay);
                    }
                    else
                    {
                        Log?.LogWarning("ActivityIndicator: Current window content is not a Panel, cannot add overlay");
                        tcs.TrySetResult(0);
                        return;
                    }

                    var registration = dismiss.Register(() => MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        try
                        {
                            if (currentContent is Microsoft.UI.Xaml.Controls.Panel panel)
                            {
                                panel.Children.Remove(overlay);
                            }
                            else if (currentContent is Microsoft.UI.Xaml.Controls.Grid grid)
                            {
                                grid.Children.Remove(overlay);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log?.LogError(ex, "Error removing activity indicator overlay");
                        }
                        tcs.TrySetResult(0);
                    }), true);

                    tcs.Task.ContinueWith(tt => registration.Dispose());
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
        Microsoft.Maui.Graphics.RectF? position = null,
        string? title = null, string? description = null, int defaultActionIndex = -1, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons)
    {
        var currentWindow = CurrentWindow;
        if (currentWindow == null)
        {
            Log?.LogWarning("UserInteraction.Menu: no window on which to display");
            return Task.FromResult(0);
        }

        var tcs = new TaskCompletionSource<int>();

        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var stackPanel = new Microsoft.UI.Xaml.Controls.StackPanel
            {
                Spacing = 8
            };

            if (!string.IsNullOrEmpty(description))
            {
                stackPanel.Children.Add(new Microsoft.UI.Xaml.Controls.TextBlock
                {
                    Text = description,
                    TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
                    Margin = new Microsoft.UI.Xaml.Thickness(0, 0, 0, 10)
                });
            }

            var dialog = new ContentDialog
            {
                Title = title ?? string.Empty,
                Content = stackPanel,
                XamlRoot = currentWindow.Content.XamlRoot
            };

            // Add cancel button
            if (cancelButton != null)
            {
                dialog.CloseButtonText = cancelButton;
                dialog.CloseButtonClick += (s, e) => tcs.TrySetResult(0);
            }

            // Add destroy button as secondary
            if (destroyButton != null)
            {
                dialog.SecondaryButtonText = destroyButton;
                dialog.SecondaryButtonClick += (s, e) => tcs.TrySetResult(1);
            }

            // Add other buttons
            var iAction = 2;
            foreach (var button in otherButtons)
            {
                var iActionIndex = iAction++;
                if (button != null)
                {
                    var menuButton = new Microsoft.UI.Xaml.Controls.Button
                    {
                        Content = button,
                        HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                        Margin = new Microsoft.UI.Xaml.Thickness(0, 2, 0, 2)
                    };

                    menuButton.Click += (s, e) =>
                    {
                        dialog.Hide();
                        tcs.TrySetResult(iActionIndex);
                    };

                    stackPanel.Children.Add(menuButton);

                    // Set as primary button if it's the default action
                    if (defaultActionIndex == iActionIndex && string.IsNullOrEmpty(dialog.PrimaryButtonText))
                    {
                        dialog.PrimaryButtonText = button;
                        dialog.PrimaryButtonClick += (s, e) => tcs.TrySetResult(iActionIndex);
                        stackPanel.Children.Remove(menuButton); // Remove from content since it's now a dialog button
                    }
                }
            }

            var registration = dismiss.Register(() => MainThread.InvokeOnMainThreadAsync(() =>
            {
                try
                {
                    dialog.Hide();
                }
                catch
                {
                    // Dialog might already be closed
                }
                tcs.TrySetResult(0);
            }));

            try
            {
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                Log?.LogError(ex, "Error showing menu dialog");
                tcs.TrySetResult(0);
            }
            finally
            {
                registration.Dispose();
            }
        });

        return tcs.Task;
    }

    internal static Task PlatformToast(string text, ToastStyle style = ToastStyle.Notice, ToastDuration duration = ToastDuration.Normal, ToastPosition position = ToastPosition.Bottom, int positionOffset = 20, CancellationToken? dismiss = null)
    {
        var currentWindow = CurrentWindow;
        if (currentWindow == null)
        {
            Log?.LogWarning("UserInteraction.Toast: no window on which to display");
            return Task.CompletedTask;
        }

        var tcs = new TaskCompletionSource<int>();

        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var currentContent = currentWindow.Content;
            if (currentContent == null)
            {
                tcs.TrySetResult(0);
                return;
            }

            // Create toast container
            var toastContainer = new Microsoft.UI.Xaml.Controls.Border
            {
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black),
                CornerRadius = new Microsoft.UI.Xaml.CornerRadius(8),
                Padding = new Microsoft.UI.Xaml.Thickness(16, 8, 16, 8),
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                MaxWidth = 400,
                Opacity = 0
            };

            // Set background color based on style
            switch (style)
            {
                case ToastStyle.Error:
                    toastContainer.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                    break;
                case ToastStyle.Warning:
                    toastContainer.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Orange);
                    break;
                case ToastStyle.Info:
                    toastContainer.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Blue);
                    break;
                default:
                    toastContainer.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Black);
                    break;
            }

            // Create text block
            var textBlock = new Microsoft.UI.Xaml.Controls.TextBlock
            {
                Text = text,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center
            };

            toastContainer.Child = textBlock;

            // Set position
            switch (position)
            {
                case ToastPosition.Top:
                    toastContainer.VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Top;
                    toastContainer.Margin = new Microsoft.UI.Xaml.Thickness(20, positionOffset, 20, 0);
                    break;
                case ToastPosition.Middle:
                    toastContainer.VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center;
                    toastContainer.Margin = new Microsoft.UI.Xaml.Thickness(20, 0, 20, 0);
                    break;
                default: // Bottom
                    toastContainer.VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Bottom;
                    toastContainer.Margin = new Microsoft.UI.Xaml.Thickness(20, 0, 20, positionOffset);
                    break;
            }

            // Add to current content
            if (currentContent is Microsoft.UI.Xaml.Controls.Panel panel)
            {
                panel.Children.Add(toastContainer);
            }
            else if (currentContent is Microsoft.UI.Xaml.Controls.Grid grid)
            {
                grid.Children.Add(toastContainer);
            }
            else
            {
                Log?.LogWarning("Toast: Current window content is not a Panel, cannot add toast");
                tcs.TrySetResult(0);
                return;
            }

            var inCall = false; // Prevent multiple calls

            void HideToast(bool animated = true)
            {
                if (inCall) return;
                inCall = true;

                if (animated)
                {
                    var fadeOut = new Microsoft.UI.Xaml.Media.Animation.DoubleAnimation
                    {
                        From = toastContainer.Opacity,
                        To = 0,
                        Duration = new Microsoft.UI.Xaml.Duration(TimeSpan.FromMilliseconds(300))
                    };

                    var storyboard = new Microsoft.UI.Xaml.Media.Animation.Storyboard();
                    storyboard.Children.Add(fadeOut);
                    Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTarget(fadeOut, toastContainer);
                    Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(fadeOut, "Opacity");

                    storyboard.Completed += (s, e) =>
                    {
                        try
                        {
                            if (currentContent is Microsoft.UI.Xaml.Controls.Panel p)
                                p.Children.Remove(toastContainer);
                            else if (currentContent is Microsoft.UI.Xaml.Controls.Grid g)
                                g.Children.Remove(toastContainer);
                        }
                        catch (Exception ex)
                        {
                            Log?.LogError(ex, "Error removing toast");
                        }
                        tcs.TrySetResult(0);
                    };

                    storyboard.Begin();
                }
                else
                {
                    try
                    {
                        if (currentContent is Microsoft.UI.Xaml.Controls.Panel p)
                            p.Children.Remove(toastContainer);
                        else if (currentContent is Microsoft.UI.Xaml.Controls.Grid g)
                            g.Children.Remove(toastContainer);
                    }
                    catch (Exception ex)
                    {
                        Log?.LogError(ex, "Error removing toast");
                    }
                    tcs.TrySetResult(0);
                }
            }

            // Handle dismiss token
            CancellationTokenRegistration? registration = null;
            if (dismiss.HasValue)
            {
                registration = dismiss.Value.Register(() => MainThread.InvokeOnMainThreadAsync(() => HideToast(false)));
            }

            // Add tap to dismiss
            toastContainer.Tapped += (s, e) => HideToast();

            // Fade in animation
            var fadeIn = new Microsoft.UI.Xaml.Media.Animation.DoubleAnimation
            {
                From = 0,
                To = 0.9,
                Duration = new Microsoft.UI.Xaml.Duration(TimeSpan.FromMilliseconds(300))
            };

            var storyboardIn = new Microsoft.UI.Xaml.Media.Animation.Storyboard();
            storyboardIn.Children.Add(fadeIn);
            Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTarget(fadeIn, toastContainer);
            Microsoft.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(fadeIn, "Opacity");

            storyboardIn.Completed += async (s, e) =>
            {
                try
                {
                    await Task.Delay((int)duration, dismiss ?? CancellationToken.None);
                    MainThread.InvokeOnMainThreadAsync(() => HideToast());
                }
                catch (TaskCanceledException)
                {
                    // Task was cancelled, toast will be hidden by dismiss token
                }
            };

            storyboardIn.Begin();

            tcs.Task.ContinueWith(t => registration?.Dispose());
        });

        return tcs.Task;
    }

}
