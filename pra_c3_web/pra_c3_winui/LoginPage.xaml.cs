using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace pra_c3_winui;

public sealed partial class LoginPage : Page
{
    public LoginPage()
    {
        this.InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorInfoBar.IsOpen = false;
        SuccessInfoBar.IsOpen = false;

        var email = EmailTextBox.Text.Trim();
        var password = PasswordBox.Password;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ErrorInfoBar.Message = "Vul alle velden in.";
            ErrorInfoBar.IsOpen = true;
            return;
        }

        LoginButton.IsEnabled = false;
        LoadingRing.IsActive = true;

        try
        {
            var response = await MainWindow.ApiService.LoginAsync(email, password);

            if (response?.Success == true && response.Player != null)
            {
                SuccessInfoBar.Message = "Succesvol ingelogd! Je kunt nu de website gebruiken.";
                SuccessInfoBar.IsOpen = true;
            }
            else
            {
                ErrorInfoBar.Message = response?.Error ?? "Inloggen mislukt. Controleer je gegevens.";
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
            LoginButton.IsEnabled = true;
            LoadingRing.IsActive = false;
        }
    }

    private async void OpenWebsite_Click(object sender, RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri("http://localhost:8000"));
    }

    private void RegisterLink_Click(object sender, RoutedEventArgs e)
    {
        if (App.MainWindow is MainWindow mainWindow)
        {
            mainWindow.NavigateTo(typeof(RegisterPage));
        }
    }
}
