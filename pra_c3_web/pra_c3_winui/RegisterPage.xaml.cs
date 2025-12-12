using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace pra_c3_winui;

public sealed partial class RegisterPage : Page
{
    public RegisterPage()
    {
        this.InitializeComponent();
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorInfoBar.IsOpen = false;
        SuccessInfoBar.IsOpen = false;

        var name = NameTextBox.Text.Trim();
        var email = EmailTextBox.Text.Trim();
        var password = PasswordBox.Password;
        var confirmPassword = ConfirmPasswordBox.Password;
        var role = GamblerRadioButton.IsChecked == true ? "gambler" : "player";

        // Validatie
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            ErrorInfoBar.Message = "Vul alle velden in.";
            ErrorInfoBar.IsOpen = true;
            return;
        }

        if (password.Length < 6)
        {
            ErrorInfoBar.Message = "Wachtwoord moet minimaal 6 tekens bevatten.";
            ErrorInfoBar.IsOpen = true;
            return;
        }

        if (password != confirmPassword)
        {
            ErrorInfoBar.Message = "Wachtwoorden komen niet overeen.";
            ErrorInfoBar.IsOpen = true;
            return;
        }

        RegisterButton.IsEnabled = false;
        LoadingRing.IsActive = true;

        try
        {
            var response = await MainWindow.ApiService.RegisterAsync(name, email, password, role);

            if (response?.Success == true && response.Player != null)
            {
                var roleText = role == "gambler" ? "Gokker" : "Speler";
                var creditsText = role == "gambler" ? " Je hebt 100 credits gekregen!" : "";

                SuccessInfoBar.Message = $"Account aangemaakt als {roleText}!{creditsText} Je kunt nu inloggen op de website.";
                SuccessInfoBar.IsOpen = true;

                // Clear form
                NameTextBox.Text = "";
                EmailTextBox.Text = "";
                PasswordBox.Password = "";
                ConfirmPasswordBox.Password = "";
                PlayerRadioButton.IsChecked = true;
            }
            else
            {
                ErrorInfoBar.Message = response?.Message ?? "Registratie mislukt. Probeer het opnieuw.";
                ErrorInfoBar.IsOpen = true;
            }
        }
        catch (Exception ex)
        {
            ErrorInfoBar.Message = $"Fout: {ex.Message}";
            ErrorInfoBar.IsOpen = true;
        }
        finally
        {
            RegisterButton.IsEnabled = true;
            LoadingRing.IsActive = false;
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
