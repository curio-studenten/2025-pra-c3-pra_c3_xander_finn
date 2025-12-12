// =============================================================================
// RegisterPage.xaml.cs - Registratiepagina voor nieuwe gebruikers
// =============================================================================
// Op deze pagina kunnen nieuwe gebruikers een account aanmaken.
// Er wordt gevalideerd dat de wachtwoorden overeenkomen en minimaal 6 tekens zijn.
// Na succesvolle registratie kan de gebruiker inloggen via de login pagina.
// =============================================================================

using Microsoft.UI.Xaml;          // Voor RoutedEventArgs en andere WinUI types
using Microsoft.UI.Xaml.Controls;  // Voor Page, Button, en andere UI controls

namespace pra_c3_winui;

/// <summary>
/// De registratiepagina waar nieuwe gebruikers een account kunnen aanmaken.
/// Bevat invoervelden voor gebruikersnaam, wachtwoord, en wachtwoord bevestiging.
/// </summary>
public sealed partial class RegisterPage : Page
{
    // ===== Constructor =====

    /// <summary>
    /// Initialiseert de registratiepagina.
    /// </summary>
    public RegisterPage()
    {
        // Initialiseer de XAML componenten (tekstboxen, buttons, etc.)
        this.InitializeComponent();
    }

    // ===== Event Handlers =====

    /// <summary>
    /// Wordt aangeroepen wanneer de gebruiker op de "Registreren" knop klikt.
    /// Valideert alle invoer en maakt een nieuw account aan indien geldig.
    /// </summary>
    /// <param name="sender">De button die het event heeft getriggerd.</param>
    /// <param name="e">Event argumenten (niet gebruikt).</param>
    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        // Verberg eventuele eerdere meldingen
        ErrorInfoBar.IsOpen = false;
        SuccessInfoBar.IsOpen = false;

        // Haal alle ingevoerde waarden op
        var username = UsernameTextBox.Text.Trim();        // Gebruikersnaam zonder spaties aan de randen
        var password = PasswordBox.Password;                // Het wachtwoord
        var confirmPassword = ConfirmPasswordBox.Password;  // Het bevestigingswachtwoord

        // ===== Validatie 1: Controleer of verplichte velden zijn ingevuld =====
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            // Toon foutmelding als een verplicht veld leeg is
            ErrorInfoBar.Message = "Vul alle velden in.";
            ErrorInfoBar.IsOpen = true;
            return;  // Stop de uitvoering
        }

        // ===== Validatie 2: Controleer of wachtwoorden overeenkomen =====
        if (password != confirmPassword)
        {
            // Wachtwoorden matchen niet
            ErrorInfoBar.Message = "Wachtwoorden komen niet overeen.";
            ErrorInfoBar.IsOpen = true;
            return;
        }

        // ===== Validatie 3: Controleer wachtwoord sterkte =====
        if (password.Length < 6)
        {
            // Wachtwoord is te kort
            ErrorInfoBar.Message = "Wachtwoord moet minimaal 6 tekens zijn.";
            ErrorInfoBar.IsOpen = true;
            return;
        }

        // ===== Probeer de gebruiker te registreren via DataService =====
        if (App.DataService.Register(username, password))
        {
            // Registratie succesvol!
            SuccessInfoBar.Message = "Account aangemaakt! Je kunt nu inloggen.";
            SuccessInfoBar.IsOpen = true;

            // Maak de invoervelden leeg zodat de pagina klaar is voor een nieuwe registratie
            UsernameTextBox.Text = "";
            PasswordBox.Password = "";
            ConfirmPasswordBox.Password = "";
        }
        else
        {
            // Registratie mislukt - waarschijnlijk bestaat de gebruikersnaam al
            ErrorInfoBar.Message = "Gebruikersnaam bestaat al.";
            ErrorInfoBar.IsOpen = true;
        }
    }

    /// <summary>
    /// Wordt aangeroepen wanneer de gebruiker op "Inloggen" klikt.
    /// Navigeert terug naar de login pagina.
    /// </summary>
    /// <param name="sender">De hyperlink die het event heeft getriggerd.</param>
    /// <param name="e">Event argumenten (niet gebruikt).</param>
    private void LoginLink_Click(object sender, RoutedEventArgs e)
    {
        // Navigeer terug naar de login pagina
        if (App.MainWindow is MainWindow mainWindow)
        {
            mainWindow.NavigateTo(typeof(LoginPage));
        }
    }
}
