﻿using System.Windows.Input;
using Vapolia.UserInteraction;

namespace UserInteractionDemo;

public class MainViewModel
{
    public ICommand AlertCommand { get; }
    public ICommand MenuCommand { get; }
    public ICommand ToastCommand { get; }
    public ICommand ConfirmCommand { get; }
    public ICommand Confirm3Command { get; }

    public MainViewModel()
    {
        AlertCommand = new Command(async () =>
        {
            await UserInteraction.Alert("This is a message", "Optional title", "OK");
        });

        MenuCommand = new Command(async () =>
        {
            var result = await UserInteraction.Menu("Optional title", "Optional description", "Cancel", "Delete", "Option 1", "Option 2", "Option 3");
            var text = result switch
            {
                0 => "Cancelled",
                1 => "Deleted !",
                2 => "Option 1",
                3 => "Option 2",
                4 => "Option 3",
                _ => throw new NotSupportedException()
            };

            await UserInteraction.Alert(text, "Selected item is:", "Close");
        });

        ToastCommand = new Command(async () =>
        {
            await UserInteraction.Toast("This is a message", ToastStyle.Warning, ToastDuration.Normal, ToastPosition.Bottom, 100);
        });

        ConfirmCommand = new Command(async () =>
        {
            var ok = await UserInteraction.Confirm("Are you sure you want to do that?", "Really do that?", okButton: "Yes", cancelButton: "No");
            await UserInteraction.Toast(ok ? "OK, Done" : "Cancelled", ToastStyle.Notice);
        });

        Confirm3Command = new Command(async () =>
        {
            var ok = await UserInteraction.ConfirmThreeButtons("Are you sure you want to do that?", "Really do that?", "I'm sure", "No!", "I don't know");
            var text = ok switch
            {
                ConfirmThreeButtonsResponse.Negative => "Cancelled",
                ConfirmThreeButtonsResponse.Positive => "Done!",
                ConfirmThreeButtonsResponse.Neutral => "I'll ask you later!",
                _ => throw new NotSupportedException()
            };
            await UserInteraction.Toast(text, ToastStyle.Notice);
        });
    }
}