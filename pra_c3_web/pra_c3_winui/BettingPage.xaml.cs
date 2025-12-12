using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace pra_c3_winui;

public sealed partial class BettingPage : Page
{
    private List<ApiMatch> _matches = new();
    private ApiMatch? _selectedMatch;

    public BettingPage()
    {
        this.InitializeComponent();
        LoadMatches();

        // Update potential win when amount or selection changes
        AmountBox.ValueChanged += (s, e) => UpdatePotentialWin();
        Team1WinRadio.Checked += (s, e) => UpdatePotentialWin();
        Team2WinRadio.Checked += (s, e) => UpdatePotentialWin();
        DrawRadio.Checked += (s, e) => UpdatePotentialWin();
    }

    private async void LoadMatches()
    {
        LoadingRing.IsActive = true;
        NoMatchesText.Visibility = Visibility.Collapsed;
        MatchesListView.Visibility = Visibility.Visible;

        try
        {
            _matches = await App.ApiService.GetMatchesAsync();
            MatchesListView.ItemsSource = _matches;

            if (_matches.Count == 0)
            {
                NoMatchesText.Visibility = Visibility.Visible;
                MatchesListView.Visibility = Visibility.Collapsed;
            }
        }
        catch
        {
            NoMatchesText.Text = "Fout bij laden van wedstrijden.";
            NoMatchesText.Visibility = Visibility.Visible;
            MatchesListView.Visibility = Visibility.Collapsed;
        }
        finally
        {
            LoadingRing.IsActive = false;
        }
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        LoadMatches();
    }

    private void MatchesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MatchesListView.SelectedItem is ApiMatch match)
        {
            _selectedMatch = match;
            SelectedMatchText.Text = match.DisplayMatch;
            Team1WinText.Text = $"{match.Team1Name} wint";
            Team2WinText.Text = $"{match.Team2Name} wint";

            // Check if already bet on this match
            if (App.DataService.HasBetOnMatch(match.Id))
            {
                BetOptionsPanel.Visibility = Visibility.Collapsed;
                AlreadyBetText.Visibility = Visibility.Visible;
            }
            else
            {
                BetOptionsPanel.Visibility = Visibility.Visible;
                AlreadyBetText.Visibility = Visibility.Collapsed;
                Team1WinRadio.IsChecked = true;
                UpdatePotentialWin();
            }
        }
    }

    private decimal GetSelectedOdds()
    {
        if (DrawRadio.IsChecked == true) return 3.5m;
        return 2.0m; // Team1 or Team2 win
    }

    private void UpdatePotentialWin()
    {
        var amount = (decimal)AmountBox.Value;
        var odds = GetSelectedOdds();
        var potentialWin = amount * odds;
        PotentialWinText.Text = $"€{potentialWin:F2}";
    }

    private void PlaceBetButton_Click(object sender, RoutedEventArgs e)
    {
        BetInfoBar.IsOpen = false;

        if (_selectedMatch == null)
        {
            BetInfoBar.Severity = InfoBarSeverity.Error;
            BetInfoBar.Message = "Selecteer eerst een wedstrijd.";
            BetInfoBar.IsOpen = true;
            return;
        }

        var user = App.DataService.CurrentUser;
        if (user == null || user.IsAdmin)
        {
            BetInfoBar.Severity = InfoBarSeverity.Error;
            BetInfoBar.Message = "Alleen gokkers kunnen weddenschappen plaatsen.";
            BetInfoBar.IsOpen = true;
            return;
        }

        var amount = (decimal)AmountBox.Value;
        if (amount > user.Credits)
        {
            BetInfoBar.Severity = InfoBarSeverity.Error;
            BetInfoBar.Message = $"Niet genoeg credits. Je hebt €{user.Credits:F2}.";
            BetInfoBar.IsOpen = true;
            return;
        }

        BetType betType;
        int? predictedWinnerId;
        string predictedWinnerName;

        if (Team1WinRadio.IsChecked == true)
        {
            betType = BetType.Team1Win;
            predictedWinnerId = _selectedMatch.Team1Id;
            predictedWinnerName = _selectedMatch.Team1Name;
        }
        else if (Team2WinRadio.IsChecked == true)
        {
            betType = BetType.Team2Win;
            predictedWinnerId = _selectedMatch.Team2Id;
            predictedWinnerName = _selectedMatch.Team2Name;
        }
        else
        {
            betType = BetType.Draw;
            predictedWinnerId = null;
            predictedWinnerName = "Gelijkspel";
        }

        var odds = GetSelectedOdds();

        if (App.DataService.PlaceBet(_selectedMatch.Id, _selectedMatch.DisplayMatch, betType, predictedWinnerId, predictedWinnerName, amount, odds))
        {
            BetInfoBar.Severity = InfoBarSeverity.Success;
            BetInfoBar.Message = $"Weddenschap geplaatst! €{amount:F2} op {predictedWinnerName}";
            BetInfoBar.IsOpen = true;

            // Hide bet options and show already bet message
            BetOptionsPanel.Visibility = Visibility.Collapsed;
            AlreadyBetText.Visibility = Visibility.Visible;

            // Refresh credits in parent
            if (this.Parent is Frame frame && frame.Parent is NavigationView nav && nav.Parent is Grid grid && grid.Parent is MainPage mainPage)
            {
                mainPage.RefreshCredits();
            }
        }
        else
        {
            BetInfoBar.Severity = InfoBarSeverity.Error;
            BetInfoBar.Message = "Kon weddenschap niet plaatsen.";
            BetInfoBar.IsOpen = true;
        }
    }
}
