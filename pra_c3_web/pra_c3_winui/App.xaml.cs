using Microsoft.UI.Xaml;

namespace pra_c3_winui;

public partial class App : Application
{
    public static Window? MainWindow { get; private set; }
    public static Player? CurrentPlayer { get; set; }

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();
        MainWindow.Activate();
    }
}
