using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace pra_c3_winui;

public sealed partial class RegisterPage : Page
{
    public RegisterPage()
    {
        this.InitializeComponent();
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorInfoBar.IsOpen = false;
        SuccessInfoBar.IsOpen = false;

        var username = UsernameTextBox.Text.Trim();
        var password = PasswordBox.Password;
        var confirmPassword = ConfirmPasswordBox.Password;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ErrorInfoBar.Message = "Vul alle velden in.";
            ErrorInfoBar.IsOpen = true;
            return;
        }

        if (password != confirmPassword)
        {
            ErrorInfoBar.Message = "Wachtwoorden komen niet overeen.";
            ErrorInfoBar.IsOpen = true;
            return;
        }

        if (password.Length < 6)
        {
            ErrorInfoBar.Message = "Wachtwoord moet minimaal 6 tekens zijn.";
            ErrorInfoBar.IsOpen = true;
            return;
        }

        if (App.DataService.Register(username, password))
        {
            SuccessInfoBar.Message = "Account aangemaakt! Je kunt nu inloggen.";
            SuccessInfoBar.IsOpen = true;

            // Clear fields
            UsernameTextBox.Text = "";
            PasswordBox.Password = "";
            ConfirmPasswordBox.Password = "";
        }
        else
        {
            ErrorInfoBar.Message = "Gebruikersnaam bestaat al.";
            ErrorInfoBar.IsOpen = true;
        }
    }

    private void LoginLink_Click(object sender, RoutedEventArgs e)
    {
        if (App.MainWindow is MainWindow mainWindow)
        {
            mainWindow.NavigateTo(typeof(LoginPage));
        }
    }
}
