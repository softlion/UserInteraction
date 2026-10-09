---
name: user-interaction
description: Reference for the Vapolia.UserInteraction NuGet package in .NET MAUI. Use when the user asks to show a confirm dialog, alert, text input prompt, three-button dialog, action sheet or single choice menu, toast or snackbar, wait indicator or blocking activity indicator, or mentions UserInteraction, IUserInteraction or UseUserInteraction.
---

# Vapolia.UserInteraction for .NET MAUI

This skill is for AI assistants writing app code that uses the `Vapolia.UserInteraction` NuGet package.

## Summary

1. Add the `Vapolia.UserInteraction` NuGet package (6.x targets net10 only).
2. Call `.UseUserInteraction(() => Application.Current!.Windows[0])` in `MauiProgram.cs`.
3. On Android, set `colorSurface` in the app theme and use a MaterialComponents parent theme.
4. Call the static methods of `UserInteraction`, or inject `IUserInteraction`. Both expose the same API.

Platforms: Android, iOS, MacCatalyst, Windows. Namespace: `Vapolia.UserInteractions` (plural). The class is `UserInteraction` (singular).

## Setup

```csharp
using Vapolia.UserInteractions;

public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .UseUserInteraction(() => Application.Current!.Windows[0]);
    return builder.Build();
}
```

`UseUserInteraction(Func<Window> getWindow, ILogger<UserInteraction>? log = null)` stores the window getter used to place popups and registers `IUserInteraction` as a singleton.

Android theme (`Platforms/Android/Resources/values/styles.xml`):

```xml
<style name="MainTheme" parent="MainTheme.Base">
  <item name="colorSurface">#333333</item>
</style>
<style name="MainTheme.Base" parent="Theme.MaterialComponents.Light.DarkActionBar">
  ...
</style>
```

Android dialogs are `MaterialAlertDialog` and follow the Material theme. iOS uses `UIAlertController`.

## API

All methods run on the UI thread internally and can be called from any thread.

### Confirm, Alert, Input, ConfirmThreeButtons

```csharp
Task<bool> Confirm(string message, string? title = null, string okButton = "OK", string cancelButton = "Cancel", CancellationToken? dismiss = null);
Task Alert(string message, string title = "", string okButton = "OK");
Task<string?> Input(string message, string? defaultValue = null, string? placeholder = null, string? title = null, string okButton = "OK", string cancelButton = "Cancel", FieldType fieldType = FieldType.Default, int maxLength = 0, bool selectContent = true);
Task<ConfirmThreeButtonsResponse> ConfirmThreeButtons(string message, string? title = null, string positive = "Yes", string negative = "No", string neutral = "Maybe");
```

- `Confirm` returns `false` on cancel or when `dismiss` is cancelled.
- `Input` returns `null` on cancel. `FieldType` is `Default`, `Email`, `Integer` or `Decimal`. `maxLength = 0` means no limit. `selectContent = true` selects the default text so typing replaces it.
- `ConfirmThreeButtonsResponse` is `Positive`, `Negative` or `Neutral`.

### Menu (action sheet)

```csharp
Task<int> Menu(CancellationToken dismiss = default, bool userCanDismiss = true, RectF? position = null, string? title = null, string? description = null, int defaultActionIndex = -1, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons);
Task<int> Menu(CancellationToken dismiss, bool userCanDismiss, string? title = null, string? description = null, int defaultActionIndex = -1, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons);
Task<int> Menu(string? title = null, string? description = null, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons);
Task<int> Menu(RectF? position = null, string? title = null, string? description = null, string? cancelButton = null, string? destroyButton = null, params string[] otherButtons);
```

Return value:

| Value | Meaning |
|---|---|
| 0 | Cancel button, hardware back key, or `dismiss` cancelled. Returned even when `cancelButton` is null. |
| 1 | Destroy button. |
| 2+ | `otherButtons[result - 2]`. |

- A `null` entry in `otherButtons` is hidden but still takes an index, so indexes stay stable when items vary.
- `destroyButton` is shown in red. `cancelButton` is shown apart from the other items on iOS.
- `defaultActionIndex` ranges from 2 to 2 + number of items. Other values are ignored.
- `position` is an absolute screen rectangle (`Microsoft.Maui.Graphics.RectF`). The menu appears around it on tablets and desktops. The `Vapolia.MauiGestures` package gives this rectangle with `args.GetAbsoluteBoundsF()`.

```csharp
var choice = await UserInteraction.Menu(default, true, position, "Actions", cancelButton: "Cancel", destroyButton: "Delete", otherButtons: ["Edit", "Share"]);
if (choice == 1) { /* delete */ }
else if (choice == 2) { /* edit */ }
```

### Toast

```csharp
Task Toast(string text, ToastStyle style = ToastStyle.Notice, ToastDuration duration = ToastDuration.Normal, ToastPosition position = ToastPosition.Bottom, int positionOffset = 20, CancellationToken? dismiss = null, Color? backgroundColor = null, Color? textColor = null);
```

- The task completes when the toast disappears. Do not await it if the code must continue immediately.
- `ToastStyle`: `Custom`, `Info`, `Notice`, `Warning`, `Error`. Android tints `Warning` orange and `Error` red. Windows tints `Info` blue, `Warning` orange, `Error` red. iOS ignores the style and uses black.
- `ToastDuration`: `Short` (1 s), `Normal` (2.5 s), `Long` (8 s).
- `ToastPosition`: `Top`, `Middle`, `Bottom`.
- `positionOffset` is in dp/points. On iOS and Android it is measured from the safe area edge.
- `backgroundColor` overrides the style color. `textColor` overrides the default white text. Both are `Microsoft.Maui.Graphics.Color`.
- Cancelling `dismiss` hides the toast early. On iOS and Windows, a tap also dismisses it.

```csharp
await UserInteraction.Toast("Saved", ToastStyle.Notice);
await UserInteraction.Toast("Network error", ToastStyle.Error, ToastDuration.Long, ToastPosition.Top);
await UserInteraction.Toast("Synced", backgroundColor: Colors.DarkGreen, textColor: Colors.White);
```

### WaitIndicator

```csharp
IWaitIndicator WaitIndicator(CancellationToken dismiss, string? message = null, string? title = null, int? displayAfterSeconds = null, bool userCanDismiss = true);
```

Shows a dialog with a title, a body and an indeterminate progress bar. Cancel `dismiss` to close it. `IWaitIndicator` exposes:

- `CancellationToken UserDismissedToken`: cancelled when the user dismisses the indicator (if `userCanDismiss` is true).
- `string Title { set; }` and `string Body { set; }`: update the texts while displayed.

```csharp
using var cts = new CancellationTokenSource();
try
{
    var wait = UserInteraction.WaitIndicator(cts.Token, "Signing in", "Please wait");
    await LoginAsync(wait.UserDismissedToken);
}
finally
{
    cts.Cancel();
}
```

### ActivityIndicator

```csharp
Task ActivityIndicator(CancellationToken dismiss, double? apparitionDelay = null, uint? argbColor = null);
```

Shows a full screen spinner that blocks user interaction until `dismiss` is cancelled. `apparitionDelay` is in seconds. User interaction is not blocked during this delay. `argbColor` is `0xAARRGGBB`.

Set a default spinner color for iOS and Windows with `UserInteraction.DefaultColor = Colors.Orange;` (`Color?`). On Android, use the theme instead.

## Static or injected

```csharp
// static
var ok = await UserInteraction.Confirm("Delete this item?");

// dependency injection
public class MyViewModel(IUserInteraction ui)
{
    public async Task Save()
    {
        if (await ui.Confirm("Save changes?"))
            await ui.Toast("Saved");
    }
}
```

Prefer `IUserInteraction` in view models so they can be unit tested with a mock.

## Troubleshooting

| Symptom | Fix |
|---|---|
| Nothing is shown, or `NullReferenceException` on the window | `UseUserInteraction` is missing in `MauiProgram.cs`, or the window getter returns null. |
| Android crash when inflating a dialog or snackbar | The app theme is not a `Theme.MaterialComponents` descendant, or `colorSurface` is missing. |
| `NotSupportedException` "Can't run on non platform specific code" | The code runs in a plain `net10.0` target (unit tests). Inject `IUserInteraction` and mock it. |
| Type `UserInteraction` not found | Use `using Vapolia.UserInteractions;` (plural) since 5.1.0. |
