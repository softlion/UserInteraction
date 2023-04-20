# User Interaction for Maui and Xamarin

Compatibility: Android, iOS, Maui, Xamarin

Nuget (both Xamarin and Maui)
[![NuGet Maui](https://img.shields.io/nuget/v/Vapolia.UserInteraction.Maui.svg?style=for-the-badge)](https://www.nuget.org/packages/Vapolia.UserInteraction.Maui/)  
![Nuget](https://img.shields.io/nuget/dt/Vapolia.UserInteraction.Maui)

Legacy Nuget for Xamarin, not updated anymore
[![NuGet Xamarin](https://img.shields.io/nuget/v/Vapolia.UserInteraction.svg?style=for-the-badge)](https://www.nuget.org/packages/Vapolia.UserInteraction/)  
![Nuget](https://img.shields.io/nuget/dt/Vapolia.UserInteraction)


## Examples
```csharp
//confirm
var ok = await Vapolia.UserInteraction.Confirm("Are you sure?");

//display a wait indicator while waiting for a long mandatory operation to complete
var dismiss = new CancellationTokenSource();
try 
{
  var userCancelled = Vapolia.UserInteraction.WaitIndicator(dismiss.Token, "Please wait", "Loggin in");
  await Task.Delay(3000, userCancelled); //simulate a long operation
} 
finally 
{
  dismiss.Cancel();
}

//display an obtrusive loading indicator
var dismiss = new CancellationTokenSource();
try 
{
  await Vapolia.UserInteraction.ActivityIndicator(dismiss.Token, apparitionDelay: 0.5, argbColor: (uint)0xFFFFFF);
  await Task.Delay(3000); //simulate a long operation
} 
finally 
{
  dismiss.Cancel();
}


//Single choice menu with optional cancel and destroy items
var cancel = default(CancellationToken); //you can cancel the dialog programatically.
var menu = await ui.Menu(cancel, true, "Choose something", "Cancel", null, "item1", "item2", ...); //You can add as many items as your want
//returns:
//0 => always cancel action (even if not displayed, ie the cancel text is null)
//1 => always destroy action (even if not displayed, ie the destroy text is null)
//2+ => item1, item2, ...
if (menu >= 2)
{
}
```

## Documentation

### Single choice menu
Menu: standard action sheet with single item choice  
UIAlertController on iOS. MaterialAlertDialog on android.


```csharp
Task<int> Menu(CancellationToken dismiss, bool userCanDismiss, string? title, string description, int defaultActionIndex, string cancelButton, string destroyButton, params string[] otherButtons);
Task<int> Menu(CancellationToken dismiss, bool userCanDismiss, string? title, string cancelButton, string? destroyButton, params string[] otherButtons);
```
cancel and destroy buttons are optional, and are displayed differently on iOS.  
destroy is in red, cancel is separated from the other buttons.  
This is the best UI practice, don't try to change it.

### Wait indicators with or without progress
```csharp
//Displays a wait indicator (title + body + indeterminate progress bar)
//Returns a controller for the wait indicator
IWaitIndicator WaitIndicator(CancellationToken dismiss, string message = null, string title=null, int? displayAfterSeconds = null, bool userCanDismiss = true);

//Display an activity indicator which blocks user interaction.
Task ActivityIndicator(CancellationToken dismiss, double? apparitionDelay = null, uint? argbColor = null);
```

### Confirmation prompts and alerts

A `Toast` is an unobtrusive temporary tooltip-like text used to confirm that an action was done succesfully or failed.
An `Input` is an alert popup with one text field. You can choose the keyboard type to limit to numbers for example.  
Confirm: native dialog with 2 buttons (mostly used for ok/cancel)  
ConfirmThreeButtons: native dialog with 3 choices. Not Task friendly.

```csharp
Task<bool> Confirm(string message, string title = null, string okButton = "OK", string cancelButton = "Cancel", CancellationToken? dismiss = null);

void ConfirmThreeButtons(string message, Action<ConfirmThreeButtonsResponse> answer, string title = null, string positive = "Yes", string negative = "No",
    string neutral = "Maybe");

Task Alert(string message, string title = "", string okButton = "OK");

Task Toast(string text, ToastStyle style = ToastStyle.Notice, ToastDuration duration = ToastDuration.Normal, ToastPosition position = ToastPosition.Bottom, int positionOffset = 20, CancellationToken? dismiss = null);

Task<string?> Input(string message, string defaultValue = null, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", FieldType fieldType = FieldType.Default, int maxLength = 0, bool selectContent = true);
```

If `selectContent` is `true` (default), the text is automatically selected, so when the user starts typing it is replaced.

### Setup

On Android only, add `colorSurface` to your application's style:

```xml
  <style name="MainTheme" parent="MainTheme.Base">

    <!-- Required -->
    <item name="colorSurface">#333333</item>

  </style>
```

### Theme

#### iOS
set a default color for all activity indicators:

```csharp
Vapolia.UserInteraction.DefaultColor = 0xAARRGGBB;
```

#### Android
This lib uses a standard `Android.Material.Dialog` that is themed by the material theme as described in the [Google documentation](https://material.io/components/dialogs/android#theming-dialogs). 

Also check [this stackoverflow answer](https://stackoverflow.com/questions/52829954/materialcomponents-theme-alert-dialog-buttons/59110804#59110804) for a sample.

### Maui / Xamarin Forms

To get the tapped item's rectangle, use [this gist](https://gist.github.com/softlion/5a845180c51b90c8624187273cef9193)

## About

License: MIT

Enterprise support available, contact [Vapolia](https://vapolia.eu) through the live chat.
