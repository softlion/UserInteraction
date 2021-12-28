using System;
using System.Collections.Generic;
using TheDemo.ViewModels;
using TheDemo.Views;
using Xamarin.Forms;

namespace TheDemo
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
        }

    }
}
