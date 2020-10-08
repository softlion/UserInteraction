UserInteraction Plugin for Xamarin
==================================

For: Android, iOS.
Framework independant: works on Xamarin Forms, Xamarin native, Mvvmcross, ...

All features are async and uses Xamarin.Essentials to get the display context.
Uses C# Nullables.

[![NuGet](https://img.shields.io/nuget/v/Vapolia.UserInteraction.svg?style=for-the-badge)](https://www.nuget.org/packages/Vapolia.UserInteraction/)  
![Nuget](https://img.shields.io/nuget/dt/Vapolia.UserInteraction)

### Features

#### Single choice menu
Menu: standard action sheet with single item choice  
UIAlertController on iOS. AlertDialog on android.


```csharp
Task<int> Menu(CancellationToken dismiss, bool userCanDismiss, string? title, string description, int defaultActionIndex, string cancelButton, string destroyButton, params string[] otherButtons);
Task<int> Menu(CancellationToken dismiss, bool userCanDismiss, string? title, string cancelButton, string? destroyButton, params string[] otherButtons);
```
cancel and destroy buttons are optional, and are displayed differently on iOS.  
destroy is in red, cancel is separated from the other buttons.  
This is the best UI practice, don't try to change it. 


#### Wait indicators with or without progress
```csharp
//Displays a wait indicator (title + body + indeterminate progress bar)
//Returns a controller for the wait indicator
IWaitIndicator WaitIndicator(CancellationToken dismiss, string message = null, string title=null, int? displayAfterSeconds = null, bool userCanDismiss = true);

//Display an activity indicator which blocks user interaction.
Task ActivityIndicator(CancellationToken dismiss, double? apparitionDelay = null, uint? argbColor = null);
```

#### Confirmation prompts and alerts

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

Task<string?> Input(string message, string defaultValue = null, string placeholder = null, string title = null, string okButton = "OK", string cancelButton = "Cancel", FieldType fieldType = FieldType.Default, int maxLength = 0);
```

#### Examples
```csharp
//confirm
	var ui = new UserInteraction>();
	var ok = await ui.Confirm("Are you sure?");

//wait for an operation to complete
    var dismiss = new CancellationTokenSource();
    var userCancelled = ui.WaitIndicator(dismiss.Token, "Please wait", "Loggin in");
    await Task.Delay(3000, userCancelled); //simulate a long operation
    dismiss.Cancel();

//display an obtrusive loading indicator
    var dismiss = new CancellationTokenSource();
	await ui.ActivityIndicator(dismiss.Token, apparitionDelay: 0.5, argbColor: (uint)0xFFFFFF);
    await Task.Delay(3000); //simulate a long operation
    dismiss.Cancel();


//Single choice menu with optional cancel and destroy items
	var ui = Mvx.IoCProvider.Resolve<IUserInteraction>();
    var cancel = default(CancellationToken); //you can cancel the dialog programatically. Menu will return 0.
    var menu = await ui.Menu(cancel, true, "Choose something", "Cancel", null, "item1", "item2"); //You can add as many items as your want
    //returns:
    //0 => always cancel action (even if not displayed, ie the cancel text is null)
    //1 => always destroy action (even if not displayed, ie the destroy text is null)
    //2+ => item1, item2, ...
    if (menu >= 2)
	{
	}
```

### Theming

Set default color for all activity indicators:

```csharp
uint DefaultColor { set; }
```

#### On Android

Create a theme

```xml
<style name="MyAlertDialogStyle" parent="Theme.AppCompat.Light.Dialog.Alert">
   <!-- Used for the buttons -->
   <item name="colorAccent">#FFC107</item>
   <!-- Used for the title and text -->
   <item name="android:textColorPrimary">#FFFFFF</item>
   <!-- Used for the background -->
   <item name="android:background">#4CAF50</item>
</style>

In order to change the Appearance of the Title, you can do the following. First add a new style:

<style name="MyTitleTextStyle">
   <item name="android:textColor">#FFEB3B</item>
   <item name="android:textAppearance">@style/TextAppearance.AppCompat.Title</item>
</style>
afterwards simply reference this style in your MyAlertDialogStyle:

<style name="MyAlertDialogStyle" parent="Theme.AppCompat.Light.Dialog.Alert">
   ...
   <item name="android:windowTitleStyle">@style/MyTitleTextStyle</item>
</style>    
```

Apply theme

```
var ui = new UserInteraction();
ui.ThemeResId = Resource.Style.MyAlertDialog;
```
