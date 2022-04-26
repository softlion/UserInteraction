﻿using System;
using System.Windows.Input;
using Vapolia.UserInteraction;
using Xamarin.Forms;

namespace TheDemo.ViewModels
{
    public class AboutViewModel
    {
        public ICommand AlertCommand { get; }
        public ICommand MenuCommand { get; }
        public object ToastCommand { get; }
        public object ConfirmCommand { get; }
        public object Confirm3Command { get; }

        public AboutViewModel()
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
                };

                await UserInteraction.Alert(text, "Selected item is:", "Close");
            });

            ToastCommand = new Command(async () =>
            {
                await UserInteraction.Toast("This is a message", ToastStyle.Warning);
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
                    ConfirmThreeButtonsResponse.Neutral => "I'll ask you later!"
                };
                await UserInteraction.Toast(text, ToastStyle.Notice);
            });
        }
    }
}