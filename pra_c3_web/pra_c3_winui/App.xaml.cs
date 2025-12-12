using Microsoft.UI.Xaml;

namespace pra_c3_winui;

public partial class App : Application
{
    public static Window? MainWindow { get; private set; }
    public static ApiService ApiService { get; } = new ApiService();
    public static LocalDataService DataService { get; } = new LocalDataService();

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
