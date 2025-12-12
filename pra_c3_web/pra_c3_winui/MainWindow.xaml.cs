using Microsoft.UI.Xaml;

namespace pra_c3_winui;

public sealed partial class MainWindow : Window
{
    public static ApiService ApiService { get; } = new ApiService();

    public MainWindow()
    {
        this.InitializeComponent();
        this.Title = "⚽ Schoolvoetbal - WinUI App";

        // Start met de login pagina
        ContentFrame.Navigate(typeof(LoginPage));
    }

    public void NavigateTo(Type pageType)
    {
        ContentFrame.Navigate(pageType);
    }

    public void NavigateToMainPage()
    {
        ContentFrame.Navigate(typeof(MainPage));
    }
}
