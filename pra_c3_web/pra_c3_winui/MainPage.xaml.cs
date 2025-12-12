// =============================================================================
// MainPage.xaml.cs - Hoofdpagina met navigatie naar alle functies
// =============================================================================
// Dit is de centrale hub van de applicatie waar ingelogde gebruikers
// kunnen navigeren naar de verschillende onderdelen:
// - Wedden: Bekijk wedstrijden en plaats weddenschappen
// - Mijn Weddenschappen: Bekijk je geplaatste weddenschappen
// - Resultaten: Bekijk afgelopen wedstrijden en scores
// =============================================================================

using Microsoft.UI.Xaml;          // Voor RoutedEventArgs en andere WinUI types
using Microsoft.UI.Xaml.Controls;  // Voor Page, NavigationView, en andere UI controls

namespace pra_c3_winui;

/// <summary>
/// De hoofdpagina met een NavigationView voor navigatie tussen de verschillende secties.
/// Toont ook informatie over de ingelogde gebruiker en hun saldo.
/// </summary>
public sealed partial class MainPage : Page
{
    // ===== Constructor =====

    /// <summary>
    /// Initialiseert de hoofdpagina en toont de gebruikersinformatie.
    /// </summary>
    public MainPage()
    {
        // Initialiseer de XAML componenten
        this.InitializeComponent();

        // Toon de gebruikersinformatie (naam en credits)
        UpdateUserInfo();

        // Selecteer het eerste menu item standaard (Wedden)
        // Dit zorgt ervoor dat de BettingPage direct wordt geladen
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    // ===== Private helper methodes =====

    /// <summary>
    /// Update de weergave van gebruikersinformatie in de UI.
    /// Toont de gebruikersnaam en het huidige saldo.
    /// </summary>
    private void UpdateUserInfo()
    {
        // Haal de huidige gebruiker op uit de DataService
        var user = App.DataService.CurrentUser;

        if (user != null)
        {
            // Toon "Admin: naam" of "Gokker: naam" afhankelijk van het type gebruiker
            UserInfoText.Text = user.IsAdmin ? $"Admin: {user.Username}" : $"Gokker: {user.Username}";

            // Toon het saldo alleen voor gokkers (admins hebben geen credits)
            // F2 formatteert het getal met 2 decimalen (bijv. "100.00")
            CreditsText.Text = user.IsAdmin ? "" : $"{user.Credits:F2}";
        }
    }

    // ===== Event Handlers =====

    /// <summary>
    /// Wordt aangeroepen wanneer de gebruiker een ander menu item selecteert.
    /// Navigeert naar de bijbehorende pagina.
    /// </summary>
    /// <param name="sender">De NavigationView die het event triggert.</param>
    /// <param name="args">Event argumenten met informatie over de selectie.</param>
    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        // Controleer of het geselecteerde item een NavigationViewItem is
        if (args.SelectedItem is NavigationViewItem item)
        {
            // Haal de Tag op (deze is ingesteld in XAML en identificeert de pagina)
            var tag = item.Tag?.ToString();

            // Navigeer naar de juiste pagina op basis van de Tag
            switch (tag)
            {
                case "betting":
                    // Navigeer naar de pagina waar weddenschappen geplaatst kunnen worden
                    ContentFrame.Navigate(typeof(BettingPage));
                    break;

                case "mybets":
                    // Navigeer naar het overzicht van eigen weddenschappen
                    ContentFrame.Navigate(typeof(MyBetsPage));
                    break;

                case "results":
                    // Navigeer naar de pagina met wedstrijdresultaten
                    ContentFrame.Navigate(typeof(ResultsPage));
                    break;
            }
        }
    }

    /// <summary>
    /// Wordt aangeroepen wanneer de gebruiker op "Uitloggen" klikt.
    /// Logt de gebruiker uit en navigeert terug naar de login pagina.
    /// </summary>
    /// <param name="sender">De button die het event triggert.</param>
    /// <param name="e">Event argumenten (niet gebruikt).</param>
    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        // Log de gebruiker uit via de DataService
        App.DataService.Logout();

        // Navigeer terug naar de login pagina
        if (App.MainWindow is MainWindow mainWindow)
        {
            mainWindow.NavigateTo(typeof(LoginPage));
        }
    }

    // ===== Public methodes =====

    /// <summary>
    /// Vernieuwt de weergave van het gebruikerssaldo.
    /// Wordt aangeroepen vanuit andere pagina's wanneer het saldo verandert
    /// (bijv. na het plaatsen van een weddenschap of het winnen).
    /// </summary>
    public void RefreshCredits()
    {
        // Herlaad de gebruikersinformatie om het nieuwe saldo te tonen
        UpdateUserInfo();
    }
}
