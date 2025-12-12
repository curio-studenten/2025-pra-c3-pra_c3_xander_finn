using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace pra_c3_winui;

public sealed partial class MatchesPage : Page
{
    private List<Match> _matches = new();

    public MatchesPage()
    {
        this.InitializeComponent();
        this.Loaded += MatchesPage_Loaded;
    }

    private async void MatchesPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Toon admin info als gebruiker admin is
        if (App.CurrentPlayer?.Admin == true)
        {
            AdminInfoBar.IsOpen = true;
        }

        await LoadMatchesAsync();
    }

    private async Task LoadMatchesAsync()
    {
        LoadingRing.IsActive = true;
        RefreshButton.IsEnabled = false;

        try
        {
            _matches = await MainWindow.ApiService.GetMatchesAsync();

            if (_matches.Count > 0)
            {
                MatchesListView.ItemsSource = _matches;
                MatchesListView.Visibility = Visibility.Visible;
                EmptyState.Visibility = Visibility.Collapsed;
            }
            else
            {
                MatchesListView.Visibility = Visibility.Collapsed;
                EmptyState.Visibility = Visibility.Visible;
            }
        }
        catch
        {
            MatchesListView.Visibility = Visibility.Collapsed;
            EmptyState.Visibility = Visibility.Visible;
        }
        finally
        {
            LoadingRing.IsActive = false;
            RefreshButton.IsEnabled = true;
        }
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await LoadMatchesAsync();
    }

    private async void MatchesListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        // Alleen admin kan scores invoeren
        if (App.CurrentPlayer?.Admin != true)
        {
            return;
        }

        if (e.ClickedItem is Match match)
        {
            await ShowEditScoreDialogAsync(match);
        }
    }

    private async Task ShowEditScoreDialogAsync(Match match)
    {
        var dialog = new ContentDialog
        {
            Title = $"Score Invoeren: {match.DisplayMatch}",
            PrimaryButtonText = "Opslaan",
            CloseButtonText = "Annuleren",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        var panel = new StackPanel { Spacing = 16 };

        var scorePanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16, HorizontalAlignment = HorizontalAlignment.Center };

        var score1Box = new NumberBox
        {
            Header = match.Team1Name,
            Value = match.ScoreTeam1 ?? 0,
            Minimum = 0,
            SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Inline,
            Width = 120
        };

        var vsText = new TextBlock
        {
            Text = "-",
            FontSize = 24,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(0, 0, 0, 8)
        };

        var score2Box = new NumberBox
        {
            Header = match.Team2Name,
            Value = match.ScoreTeam2 ?? 0,
            Minimum = 0,
            SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Inline,
            Width = 120
        };

        scorePanel.Children.Add(score1Box);
        scorePanel.Children.Add(vsText);
        scorePanel.Children.Add(score2Box);

        panel.Children.Add(scorePanel);

        var infoText = new TextBlock
        {
            Text = "Winst = 3 punten, Gelijkspel = 1 punt, Verlies = 0 punten",
            FontSize = 12,
            Opacity = 0.7,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        panel.Children.Add(infoText);

        dialog.Content = panel;

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            var success = await MainWindow.ApiService.UpdateMatchScoreAsync(
                match.Id,
                (int)score1Box.Value,
                (int)score2Box.Value
            );

            if (success)
            {
                await LoadMatchesAsync();
            }
            else
            {
                var errorDialog = new ContentDialog
                {
                    Title = "Fout",
                    Content = "Kon de score niet opslaan. Probeer het opnieuw.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }
    }
}
