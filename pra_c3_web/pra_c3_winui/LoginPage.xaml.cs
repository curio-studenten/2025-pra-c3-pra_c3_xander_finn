using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace pra_c3_winui;

public sealed partial class LoginPage : Page
{
    public LoginPage()
    {
        this.InitializeComponent();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorInfoBar.IsOpen = false;
        SuccessInfoBar.IsOpen = false;

        var username = UsernameTextBox.Text.Trim();
        var password = PasswordBox.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ErrorInfoBar.Message = "Vul alle velden in.";
            ErrorInfoBar.IsOpen = true;
            return;
        }

        if (App.DataService.Login(username, password))
        {
            SuccessInfoBar.Message = "Succesvol ingelogd!";
            SuccessInfoBar.IsOpen = true;

            // Navigate to main page
            if (App.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToMainPage();
            }
        }
        else
        {
            ErrorInfoBar.Message = "Ongeldige gebruikersnaam of wachtwoord.";
            ErrorInfoBar.IsOpen = true;
        }
    }

    private void RegisterLink_Click(object sender, RoutedEventArgs e)
    {
        if (App.MainWindow is MainWindow mainWindow)
        {
            mainWindow.NavigateTo(typeof(RegisterPage));
        }
    }
}
