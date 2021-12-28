using TheDemo.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheDemo
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }
}
