// =============================================================================
// MyBetsPage.xaml.cs - Overzicht van de weddenschappen van de gebruiker
// =============================================================================
// Op deze pagina kan de gebruiker al hun geplaatste weddenschappen bekijken.
// De pagina toont:
// - Alle weddenschappen (lopend, gewonnen, verloren)
// - Totaal bedrag ingezet
// - Totaal gewonnen
// - Aantal lopende weddenschappen
//
// Bij het laden worden resultaten opgehaald van de API om weddenschappen
// automatisch te verwerken (controleren of ze gewonnen of verloren zijn).
// =============================================================================

using Microsoft.UI.Xaml;          // Voor RoutedEventArgs en andere WinUI types
using Microsoft.UI.Xaml.Controls;  // Voor Page, ListView, en andere UI controls

namespace pra_c3_winui;

/// <summary>
/// De pagina die een overzicht toont van alle weddenschappen van de huidige gebruiker.
/// Verwerkt automatisch resultaten bij het laden om te controleren of weddenschappen gewonnen zijn.
/// </summary>
public sealed partial class MyBetsPage : Page
{
    // ===== Constructor =====

    /// <summary>
    /// Initialiseert de pagina en laadt de weddenschappen.
    /// </summary>
    public MyBetsPage()
    {
        // Initialiseer de XAML componenten
        this.InitializeComponent();

        // Laad de weddenschappen en controleer de API voor resultaten
        LoadBetsAndCheckResults();
    }

    // ===== Private methodes =====

    /// <summary>
    /// Laadt de weddenschappen en controleert de API voor nieuwe resultaten.
    /// Als er resultaten zijn, worden de lopende weddenschappen automatisch verwerkt.
    /// Dit zorgt ervoor dat weddenschappen correct worden bijgewerkt naar "Gewonnen" of "Verloren".
    /// </summary>
    private async void LoadBetsAndCheckResults()
    {
        // Toon de loading indicator
        LoadingRing.IsActive = true;

        try
        {
            // ===== Stap 1: Haal resultaten op van de API =====
            // Dit zijn alle wedstrijden die al gespeeld zijn
            var results = await App.ApiService.GetResultsAsync();

            // ===== Stap 2: Verwerk elke uitslag =====
            // Controleer voor elk resultaat of de gebruiker een weddenschap had
            // en of die weddenschap correct was
            foreach (var result in results)
            {
                // ProcessBetResult controleert alle weddenschappen voor deze wedstrijd
                // en update de status naar Won of Lost op basis van de uitslag
                App.DataService.ProcessBetResult(result.Id, result.WinnerId);
            }
        }
        catch
        {
            // Als de API niet bereikbaar is, toon gewoon de bestaande weddenschappen
            // zonder ze te verwerken. De status blijft dan "Lopend".
        }
        finally
        {
            // Verberg de loading indicator (altijd, ook bij fouten)
            LoadingRing.IsActive = false;
        }

        // ===== Stap 3: Toon de weddenschappen =====
        LoadBets();

        // ===== Stap 4: Vernieuw het saldo in de hoofdpagina =====
        // Dit is nodig omdat gewonnen weddenschappen het saldo verhogen
        RefreshMainPageCredits();
    }

    /// <summary>
    /// Laadt en toont de weddenschappen van de huidige gebruiker.
    /// Update ook de statistieken (totaal ingezet, gewonnen, lopend).
    /// </summary>
    private void LoadBets()
    {
        // Haal alle weddenschappen van de huidige gebruiker op
        var bets = App.DataService.GetUserBets();

        // Koppel de weddenschappen aan de ListView voor weergave
        BetsListView.ItemsSource = bets;

        // ===== Toon de juiste UI op basis van of er weddenschappen zijn =====
        if (bets.Count == 0)
        {
            // Geen weddenschappen - toon melding, verberg lijst
            NoBetsText.Visibility = Visibility.Visible;
            BetsListView.Visibility = Visibility.Collapsed;
        }
        else
        {
            // Wel weddenschappen - verberg melding, toon lijst
            NoBetsText.Visibility = Visibility.Collapsed;
            BetsListView.Visibility = Visibility.Visible;
        }

        // Update de statistieken onderaan de pagina
        UpdateTotals(bets);
    }

    /// <summary>
    /// Berekent en toont de totalen en statistieken van de weddenschappen.
    /// </summary>
    /// <param name="bets">Lijst van alle weddenschappen van de gebruiker.</param>
    private void UpdateTotals(List<Bet> bets)
    {
        // ===== Bereken totaal ingezet bedrag =====
        // Sum() telt alle Amount waarden bij elkaar op
        var totalBet = bets.Sum(b => b.Amount);

        // ===== Bereken totaal gewonnen bedrag =====
        // Filter eerst op gewonnen weddenschappen, tel dan de Payout waarden op
        // ?? 0 zorgt ervoor dat null waarden als 0 worden geteld
        var totalWon = bets.Where(b => b.Status == BetStatus.Won).Sum(b => b.Payout ?? 0);

        // ===== Bereken totaal verloren bedrag (niet gebruikt in UI maar wel berekend) =====
        var totalLost = bets.Where(b => b.Status == BetStatus.Lost).Sum(b => b.Amount);

        // ===== Tel aantal lopende weddenschappen =====
        var pending = bets.Count(b => b.Status == BetStatus.Pending);

        // ===== Update de UI met de berekende waarden =====
        // F2 formatteert naar 2 decimalen (bijv. "50.00")
        TotalBetText.Text = $"€{totalBet:F2}";
        TotalWonText.Text = $"€{totalWon:F2}";
        PendingCountText.Text = pending.ToString();
    }

    /// <summary>
    /// Probeert het saldo in de hoofdpagina te vernieuwen.
    /// Dit is nodig omdat gewonnen weddenschappen het saldo verhogen.
    /// </summary>
    private void RefreshMainPageCredits()
    {
        try
        {
            // ===== Navigeer door de visual tree naar MainPage =====
            // De structuur is: MyBetsPage -> Frame -> NavigationView -> Grid -> MainPage
            // We moeten door alle lagen heen om de RefreshCredits methode aan te roepen
            if (this.Parent is Frame frame &&
                frame.Parent is NavigationView nav &&
                nav.Parent is Grid grid &&
                grid.Parent is MainPage mainPage)
            {
                // Roep de refresh methode aan om het saldo te updaten
                mainPage.RefreshCredits();
            }
        }
        catch
        {
            // Als de navigatie faalt, negeer de fout
            // Het saldo wordt dan de volgende keer dat de pagina wordt geladen bijgewerkt
        }
    }

    // ===== Event Handlers =====

    /// <summary>
    /// Wordt aangeroepen wanneer de gebruiker op "Vernieuwen" klikt.
    /// Herlaadt de weddenschappen en controleert opnieuw op resultaten.
    /// </summary>
    /// <param name="sender">De button die het event triggert.</param>
    /// <param name="e">Event argumenten (niet gebruikt).</param>
    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        // Herlaad alles - dit controleert ook op nieuwe resultaten
        LoadBetsAndCheckResults();
    }
}
