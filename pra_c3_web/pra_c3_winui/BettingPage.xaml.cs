// =============================================================================
// BettingPage.xaml.cs - Pagina voor het plaatsen van weddenschappen
// =============================================================================
// Dit is de kern van de gok functionaliteit.
// Gebruikers kunnen hier:
// - Een lijst van beschikbare wedstrijden bekijken
// - Een wedstrijd selecteren om op te gokken
// - Een voorspelling kiezen (Team 1 wint, Team 2 wint, of Gelijkspel)
// - Een bedrag inzetten
// - De potentiele winst zien op basis van de odds
// - De weddenschap plaatsen
// =============================================================================

using Microsoft.UI.Xaml;          // Voor RoutedEventArgs en andere WinUI types
using Microsoft.UI.Xaml.Controls;  // Voor Page, ListView, en andere UI controls

namespace pra_c3_winui;

/// <summary>
/// De pagina waar gebruikers weddenschappen kunnen plaatsen op wedstrijden.
/// Bevat een lijst van wedstrijden en opties voor het plaatsen van weddenschappen.
/// </summary>
public sealed partial class BettingPage : Page
{
    // ===== Private velden =====

    /// <summary>
    /// Cache van alle geladen wedstrijden van de API.
    /// Wordt gevuld door LoadMatches() en gebruikt als ItemsSource voor de ListView.
    /// </summary>
    private List<ApiMatch> _matches = new();

    /// <summary>
    /// De momenteel geselecteerde wedstrijd waarop de gebruiker kan gokken.
    /// Nullable omdat er mogelijk nog geen wedstrijd is geselecteerd.
    /// </summary>
    private ApiMatch? _selectedMatch;

    // ===== Constructor =====

    /// <summary>
    /// Initialiseert de betting pagina en laadt de wedstrijden.
    /// </summary>
    public BettingPage()
    {
        // Initialiseer de XAML componenten
        this.InitializeComponent();

        // Laad de beschikbare wedstrijden van de API
        LoadMatches();

        // ===== Event handlers voor het berekenen van potentiele winst =====
        // Wanneer het bedrag of de selectie verandert, update de potentiele winst weergave

        // Lambda expressies worden gebruikt als event handlers
        // Deze worden aangeroepen wanneer de waarde of selectie verandert
        AmountBox.ValueChanged += (s, e) => UpdatePotentialWin();      // Bedrag veranderd
        Team1WinRadio.Checked += (s, e) => UpdatePotentialWin();       // Team 1 wint geselecteerd
        Team2WinRadio.Checked += (s, e) => UpdatePotentialWin();       // Team 2 wint geselecteerd
        DrawRadio.Checked += (s, e) => UpdatePotentialWin();           // Gelijkspel geselecteerd
    }

    // ===== Private helper methodes =====

    /// <summary>
    /// Laadt alle beschikbare wedstrijden van de API.
    /// Dit zijn wedstrijden die nog niet gespeeld zijn.
    /// </summary>
    private async void LoadMatches()
    {
        // Toon de loading indicator terwijl data wordt geladen
        LoadingRing.IsActive = true;
        NoMatchesText.Visibility = Visibility.Collapsed;
        MatchesListView.Visibility = Visibility.Visible;

        try
        {
            // Haal wedstrijden op van de API (async om de UI niet te blokkeren)
            _matches = await App.ApiService.GetMatchesAsync();

            // Koppel de wedstrijden aan de ListView voor weergave
            MatchesListView.ItemsSource = _matches;

            // Als er geen wedstrijden zijn, toon een melding
            if (_matches.Count == 0)
            {
                NoMatchesText.Visibility = Visibility.Visible;
                MatchesListView.Visibility = Visibility.Collapsed;
            }
        }
        catch
        {
            // Bij een fout (bijv. server niet bereikbaar), toon een foutmelding
            NoMatchesText.Text = "Fout bij laden van wedstrijden.";
            NoMatchesText.Visibility = Visibility.Visible;
            MatchesListView.Visibility = Visibility.Collapsed;
        }
        finally
        {
            // Verberg de loading indicator (gebeurt altijd, ook bij fouten)
            LoadingRing.IsActive = false;
        }
    }

    /// <summary>
    /// Bepaalt de odds op basis van de geselecteerde voorspelling.
    /// Team winst heeft lagere odds (2.0x) dan gelijkspel (3.5x).
    /// </summary>
    /// <returns>De odds als decimal waarde.</returns>
    private decimal GetSelectedOdds()
    {
        // Gelijkspel is lastiger te voorspellen, dus hogere odds
        if (DrawRadio.IsChecked == true) return 3.5m;

        // Team winst heeft standaard odds van 2.0
        return 2.0m;
    }

    /// <summary>
    /// Berekent en toont de potentiele winst op basis van het ingezette bedrag en de odds.
    /// Formule: potentiele winst = inzet * odds
    /// </summary>
    private void UpdatePotentialWin()
    {
        // Haal het ingezette bedrag op (NumberBox.Value is een double, casten naar decimal)
        var amount = (decimal)AmountBox.Value;

        // Haal de odds op voor de huidige selectie
        var odds = GetSelectedOdds();

        // Bereken de potentiele winst
        var potentialWin = amount * odds;

        // Toon het bedrag geformatteerd met 2 decimalen
        PotentialWinText.Text = $"€{potentialWin:F2}";
    }

    // ===== Event Handlers =====

    /// <summary>
    /// Wordt aangeroepen wanneer de gebruiker op de "Vernieuwen" knop klikt.
    /// Herlaadt de wedstrijden van de API.
    /// </summary>
    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        LoadMatches();
    }

    /// <summary>
    /// Wordt aangeroepen wanneer de gebruiker een wedstrijd selecteert in de lijst.
    /// Toont de gok opties voor de geselecteerde wedstrijd.
    /// </summary>
    /// <param name="sender">De ListView die het event triggert.</param>
    /// <param name="e">Event argumenten met de nieuwe selectie.</param>
    private void MatchesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Controleer of de selectie een geldige wedstrijd is (pattern matching)
        if (MatchesListView.SelectedItem is ApiMatch match)
        {
            // Sla de geselecteerde wedstrijd op
            _selectedMatch = match;

            // Update de UI met de wedstrijd details
            SelectedMatchText.Text = match.DisplayMatch;              // Toon "Team1 vs Team2"
            Team1WinText.Text = $"{match.Team1Name} wint";           // Update radio button tekst
            Team2WinText.Text = $"{match.Team2Name} wint";           // Update radio button tekst

            // Controleer of de gebruiker al op deze wedstrijd heeft gegokt
            if (App.DataService.HasBetOnMatch(match.Id))
            {
                // Gebruiker heeft al gegokt - verberg de gok opties
                BetOptionsPanel.Visibility = Visibility.Collapsed;
                AlreadyBetText.Visibility = Visibility.Visible;
            }
            else
            {
                // Gebruiker kan nog gokken - toon de gok opties
                BetOptionsPanel.Visibility = Visibility.Visible;
                AlreadyBetText.Visibility = Visibility.Collapsed;

                // Selecteer standaard Team 1 wint
                Team1WinRadio.IsChecked = true;

                // Update de potentiele winst berekening
                UpdatePotentialWin();
            }
        }
    }

    /// <summary>
    /// Wordt aangeroepen wanneer de gebruiker op "Plaats Weddenschap" klikt.
    /// Valideert de invoer en plaatst de weddenschap via de DataService.
    /// </summary>
    /// <param name="sender">De button die het event triggert.</param>
    /// <param name="e">Event argumenten (niet gebruikt).</param>
    private void PlaceBetButton_Click(object sender, RoutedEventArgs e)
    {
        // Verberg eventuele eerdere meldingen
        BetInfoBar.IsOpen = false;

        // ===== Validatie 1: Controleer of een wedstrijd is geselecteerd =====
        if (_selectedMatch == null)
        {
            BetInfoBar.Severity = InfoBarSeverity.Error;
            BetInfoBar.Message = "Selecteer eerst een wedstrijd.";
            BetInfoBar.IsOpen = true;
            return;
        }

        // ===== Validatie 2: Controleer of de gebruiker mag gokken =====
        var user = App.DataService.CurrentUser;
        if (user == null || user.IsAdmin)
        {
            // Admins mogen niet gokken
            BetInfoBar.Severity = InfoBarSeverity.Error;
            BetInfoBar.Message = "Alleen gokkers kunnen weddenschappen plaatsen.";
            BetInfoBar.IsOpen = true;
            return;
        }

        // ===== Validatie 3: Controleer of de gebruiker genoeg credits heeft =====
        var amount = (decimal)AmountBox.Value;
        if (amount > user.Credits)
        {
            BetInfoBar.Severity = InfoBarSeverity.Error;
            BetInfoBar.Message = $"Niet genoeg credits. Je hebt €{user.Credits:F2}.";
            BetInfoBar.IsOpen = true;
            return;
        }

        // ===== Bepaal het type weddenschap en de voorspelde winnaar =====
        BetType betType;
        int? predictedWinnerId;
        string predictedWinnerName;

        if (Team1WinRadio.IsChecked == true)
        {
            // Team 1 wint geselecteerd
            betType = BetType.Team1Win;
            predictedWinnerId = _selectedMatch.Team1Id;
            predictedWinnerName = _selectedMatch.Team1Name;
        }
        else if (Team2WinRadio.IsChecked == true)
        {
            // Team 2 wint geselecteerd
            betType = BetType.Team2Win;
            predictedWinnerId = _selectedMatch.Team2Id;
            predictedWinnerName = _selectedMatch.Team2Name;
        }
        else
        {
            // Gelijkspel geselecteerd
            betType = BetType.Draw;
            predictedWinnerId = null;  // Geen winnaar bij gelijkspel
            predictedWinnerName = "Gelijkspel";
        }

        // Haal de odds op voor de huidige selectie
        var odds = GetSelectedOdds();

        // ===== Plaats de weddenschap via DataService =====
        if (App.DataService.PlaceBet(_selectedMatch.Id, _selectedMatch.DisplayMatch, betType, predictedWinnerId, predictedWinnerName, amount, odds))
        {
            // Weddenschap succesvol geplaatst!
            BetInfoBar.Severity = InfoBarSeverity.Success;
            BetInfoBar.Message = $"Weddenschap geplaatst! €{amount:F2} op {predictedWinnerName}";
            BetInfoBar.IsOpen = true;

            // Verberg de gok opties en toon "al gegokt" bericht
            BetOptionsPanel.Visibility = Visibility.Collapsed;
            AlreadyBetText.Visibility = Visibility.Visible;

            // ===== Vernieuw het saldo in de hoofdpagina =====
            // Navigeer omhoog door de visual tree om de MainPage te vinden
            // Dit is nodig omdat het saldo is veranderd en geupdate moet worden
            if (this.Parent is Frame frame &&
                frame.Parent is NavigationView nav &&
                nav.Parent is Grid grid &&
                grid.Parent is MainPage mainPage)
            {
                mainPage.RefreshCredits();
            }
        }
        else
        {
            // Weddenschap kon niet worden geplaatst (mogelijk al gegokt op deze wedstrijd)
            BetInfoBar.Severity = InfoBarSeverity.Error;
            BetInfoBar.Message = "Kon weddenschap niet plaatsen.";
            BetInfoBar.IsOpen = true;
        }
    }
}
