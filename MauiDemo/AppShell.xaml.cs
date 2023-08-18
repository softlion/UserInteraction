namespace MauiDemo;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute($"{nameof(Child1Page)}", typeof(Child1Page));
    }
}