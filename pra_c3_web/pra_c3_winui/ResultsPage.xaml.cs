// =============================================================================
// ResultsPage.xaml.cs - Pagina met wedstrijdresultaten en doelpunten
// =============================================================================
// Op deze pagina kunnen gebruikers de uitslagen van gespeelde wedstrijden bekijken.
// Functies:
// - Toon alle wedstrijdresultaten met scores
// - Bekijk doelpunten per wedstrijd (wie scoorde en wanneer)
// - Admins kunnen weddenschappen verwerken via een speciale knop
//
// De resultaten komen van de Laravel API en worden gecached in _results.
// =============================================================================

using Microsoft.UI.Xaml;          // Voor RoutedEventArgs en andere WinUI types
using Microsoft.UI.Xaml.Controls;  // Voor Page, ListView, ContentDialog, en andere UI controls

namespace pra_c3_winui;

/// <summary>
/// De pagina die alle wedstrijdresultaten toont en details over doelpunten.
/// Admins hebben extra functionaliteit om weddenschappen te verwerken.
/// </summary>
public sealed partial class ResultsPage : Page
{
    // ===== Private velden =====

    /// <summary>
    /// Cache van alle geladen resultaten van de API.
    /// Wordt gebruikt door de ListView en bij het verwerken van weddenschappen.
    /// </summary>
    private List<ApiResult> _results = new();

    // ===== Constructor =====

    /// <summary>
    /// Initialiseert de resultaten pagina en laadt de data.
    /// </summary>
    public ResultsPage()
    {
        // Initialiseer de XAML componenten
        this.InitializeComponent();

        // Laad de resultaten van de API
        LoadResults();

        // ===== Admin functionaliteit: Verwerk weddenschappen knop =====
        // Deze knop is alleen zichtbaar voor admins
        var user = App.DataService.CurrentUser;
        ProcessBetsButton.Visibility = (user?.IsAdmin == true) ? Visibility.Visible : Visibility.Collapsed;
    }

    // ===== Private methodes =====

    /// <summary>
    /// Laadt alle wedstrijdresultaten van de API.
    /// Dit zijn wedstrijden die al gespeeld zijn met scores en winnaars.
    /// </summary>
    private async void LoadResults()
    {
        // Toon de loading indicator
        LoadingRing.IsActive = true;
        NoResultsText.Visibility = Visibility.Collapsed;
        ResultsListView.Visibility = Visibility.Visible;

        try
        {
            // Haal resultaten op van de API (async om UI niet te blokkeren)
            _results = await App.ApiService.GetResultsAsync();

            // Koppel de resultaten aan de ListView voor weergave
            ResultsListView.ItemsSource = _results;

            // Als er geen resultaten zijn, toon een melding
            if (_results.Count == 0)
            {
                NoResultsText.Visibility = Visibility.Visible;
                ResultsListView.Visibility = Visibility.Collapsed;
            }
        }
        catch
        {
            // Bij een fout, toon een foutmelding
            NoResultsText.Text = "Fout bij laden van resultaten.";
            NoResultsText.Visibility = Visibility.Visible;
            ResultsListView.Visibility = Visibility.Collapsed;
        }
        finally
        {
            // Verberg de loading indicator (altijd, ook bij fouten)
            LoadingRing.IsActive = false;
        }
    }

    // ===== Event Handlers =====

    /// <summary>
    /// Wordt aangeroepen wanneer de gebruiker op "Vernieuwen" klikt.
    /// Herlaadt de resultaten van de API.
    /// </summary>
    /// <param name="sender">De button die het event triggert.</param>
    /// <param name="e">Event argumenten (niet gebruikt).</param>
    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        LoadResults();
    }

    /// <summary>
    /// Wordt aangeroepen wanneer de selectie in de resultaten lijst verandert.
    /// Momenteel niet gebruikt, maar kan worden uitgebreid voor extra functionaliteit.
    /// </summary>
    /// <param name="sender">De ListView die het event triggert.</param>
    /// <param name="e">Event argumenten met selectie informatie.</param>
    private void ResultsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Momenteel geen actie nodig bij selectie verandering
        // Dit kan later worden uitgebreid voor gedetailleerde weergave
    }

    /// <summary>
    /// Wordt aangeroepen wanneer de gebruiker op "Bekijk Doelpunten" klikt bij een wedstrijd.
    /// Toont een dialoog met alle doelpunten van die wedstrijd.
    /// </summary>
    /// <param name="sender">De button die het event triggert.</param>
    /// <param name="e">Event argumenten (niet gebruikt).</param>
    private async void ViewGoalsButton_Click(object sender, RoutedEventArgs e)
    {
        // ===== Haal het wedstrijd ID op uit de button Tag =====
        // De Tag property wordt in XAML ingesteld op het match ID
        if (sender is Button button && button.Tag is int matchId)
        {
            // Haal de doelpunten op van de API
            var goals = await App.ApiService.GetGoalsAsync(matchId);

            if (goals.Count > 0)
            {
                // Er zijn doelpunten - toon ze in de ListView in de dialoog
                GoalsListView.ItemsSource = goals;
                GoalsListView.Visibility = Visibility.Visible;
                NoGoalsText.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Geen doelpunten (0-0 wedstrijd) - toon melding
                GoalsListView.Visibility = Visibility.Collapsed;
                NoGoalsText.Visibility = Visibility.Visible;
            }

            // Toon de dialoog met doelpunten
            // ShowAsync() wacht tot de dialoog wordt gesloten
            await GoalsDialog.ShowAsync();
        }
    }

    /// <summary>
    /// Wordt aangeroepen wanneer de admin op "Verwerk Weddenschappen" klikt.
    /// Verwerkt alle lopende weddenschappen op basis van de huidige resultaten.
    /// </summary>
    /// <param name="sender">De button die het event triggert.</param>
    /// <param name="e">Event argumenten (niet gebruikt).</param>
    private async void ProcessBetsButton_Click(object sender, RoutedEventArgs e)
    {
        // ===== Beveiligingscheck: Alleen admins =====
        var user = App.DataService.CurrentUser;
        if (user?.IsAdmin != true)
        {
            return;  // Negeer als geen admin
        }

        // ===== Verwerk alle resultaten =====
        // Voor elk resultaat worden alle bijbehorende weddenschappen verwerkt
        foreach (var result in _results)
        {
            // ProcessBetResult controleert alle weddenschappen voor deze wedstrijd
            // en update de status naar Won of Lost
            App.DataService.ProcessBetResult(result.Id, result.WinnerId);
        }

        // ===== Toon bevestigingsdialoog =====
        var dialog = new ContentDialog
        {
            Title = "Weddenschappen Verwerkt",                            // Dialoog titel
            Content = "Alle weddenschappen zijn verwerkt op basis van de huidige resultaten.",  // Bericht
            CloseButtonText = "OK",                                       // Sluit knop tekst
            XamlRoot = this.XamlRoot                                     // Nodig voor WinUI 3 dialogen
        };

        // Toon de dialoog en wacht tot de gebruiker op OK klikt
        await dialog.ShowAsync();
    }
}
