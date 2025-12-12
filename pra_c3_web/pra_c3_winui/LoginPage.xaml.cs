// =============================================================================
// LoginPage.xaml.cs - Inlogpagina voor gebruikersauthenticatie
// =============================================================================
// Deze pagina wordt als eerste getoond wanneer de app start.
// Gebruikers moeten hier inloggen met hun gebruikersnaam en wachtwoord
// om toegang te krijgen tot de gok functionaliteit.
// =============================================================================

using Microsoft.UI.Xaml;          // Voor RoutedEventArgs en andere WinUI types
using Microsoft.UI.Xaml.Controls;  // Voor Page, Button, en andere UI controls

namespace pra_c3_winui;

/// <summary>
/// De login pagina waar gebruikers kunnen inloggen met hun credentials.
/// Bevat invoervelden voor gebruikersnaam en wachtwoord, en een link naar registratie.
/// </summary>
public sealed partial class LoginPage : Page
{
    // ===== Constructor =====

    /// <summary>
    /// Initialiseert de login pagina.
    /// </summary>
    public LoginPage()
    {
        // Initialiseer de XAML componenten (tekstboxen, buttons, etc.)
        this.InitializeComponent();
    }

    // ===== Event Handlers =====

    /// <summary>
    /// Wordt aangeroepen wanneer de gebruiker op de "Inloggen" knop klikt.
    /// Valideert de invoer en probeert de gebruiker in te loggen.
    /// </summary>
    /// <param name="sender">De button die het event heeft getriggerd.</param>
    /// <param name="e">Event argumenten (niet gebruikt).</param>
    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        // Verberg eventuele eerdere foutmeldingen of succesberichten
        // InfoBar is een WinUI control voor het tonen van berichten
        ErrorInfoBar.IsOpen = false;
        SuccessInfoBar.IsOpen = false;

        // Haal de ingevoerde waarden op
        // Trim() verwijdert spaties aan het begin en einde van de gebruikersnaam
        var username = UsernameTextBox.Text.Trim();
        var password = PasswordBox.Password;  // PasswordBox heeft een Password property ipv Text

        // ===== Validatie: Controleer of alle velden zijn ingevuld =====
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            // Toon foutmelding als een veld leeg is
            ErrorInfoBar.Message = "Vul alle velden in.";
            ErrorInfoBar.IsOpen = true;
            return;  // Stop de uitvoering van de methode
        }

        // ===== Probeer in te loggen via de DataService =====
        if (App.DataService.Login(username, password))
        {
            // Login was succesvol!
            SuccessInfoBar.Message = "Succesvol ingelogd!";
            SuccessInfoBar.IsOpen = true;

            // Navigeer naar de hoofdpagina
            // Controleer of MainWindow van het juiste type is (pattern matching)
            if (App.MainWindow is MainWindow mainWindow)
            {
                mainWindow.NavigateToMainPage();
            }
        }
        else
        {
            // Login mislukt - toon foutmelding
            ErrorInfoBar.Message = "Ongeldige gebruikersnaam of wachtwoord.";
            ErrorInfoBar.IsOpen = true;
        }
    }

    /// <summary>
    /// Wordt aangeroepen wanneer de gebruiker op "Registreren" klikt.
    /// Navigeert naar de registratiepagina.
    /// </summary>
    /// <param name="sender">De hyperlink die het event heeft getriggerd.</param>
    /// <param name="e">Event argumenten (niet gebruikt).</param>
    private void RegisterLink_Click(object sender, RoutedEventArgs e)
    {
        // Navigeer naar de registratiepagina
        if (App.MainWindow is MainWindow mainWindow)
        {
            mainWindow.NavigateTo(typeof(RegisterPage));
        }
    }
}
