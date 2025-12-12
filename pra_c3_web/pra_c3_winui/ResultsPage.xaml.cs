using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace pra_c3_winui;

public sealed partial class ResultsPage : Page
{
    private List<ApiResult> _results = new();

    public ResultsPage()
    {
        this.InitializeComponent();
        LoadResults();

        // Only admins can process bets
        var user = App.DataService.CurrentUser;
        ProcessBetsButton.Visibility = (user?.IsAdmin == true) ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void LoadResults()
    {
        LoadingRing.IsActive = true;
        NoResultsText.Visibility = Visibility.Collapsed;
        ResultsListView.Visibility = Visibility.Visible;

        try
        {
            _results = await App.ApiService.GetResultsAsync();
            ResultsListView.ItemsSource = _results;

            if (_results.Count == 0)
            {
                NoResultsText.Visibility = Visibility.Visible;
                ResultsListView.Visibility = Visibility.Collapsed;
            }
        }
        catch
        {
            NoResultsText.Text = "Fout bij laden van resultaten.";
            NoResultsText.Visibility = Visibility.Visible;
            ResultsListView.Visibility = Visibility.Collapsed;
        }
        finally
        {
            LoadingRing.IsActive = false;
        }
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        LoadResults();
    }

    private void ResultsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Nothing for now
    }

    private async void ViewGoalsButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int matchId)
        {
            var goals = await App.ApiService.GetGoalsAsync(matchId);

            if (goals.Count > 0)
            {
                GoalsListView.ItemsSource = goals;
                GoalsListView.Visibility = Visibility.Visible;
                NoGoalsText.Visibility = Visibility.Collapsed;
            }
            else
            {
                GoalsListView.Visibility = Visibility.Collapsed;
                NoGoalsText.Visibility = Visibility.Visible;
            }

            await GoalsDialog.ShowAsync();
        }
    }

    private async void ProcessBetsButton_Click(object sender, RoutedEventArgs e)
    {
        var user = App.DataService.CurrentUser;
        if (user?.IsAdmin != true)
        {
            return;
        }

        // Process all results against pending bets
        foreach (var result in _results)
        {
            App.DataService.ProcessBetResult(result.Id, result.WinnerId);
        }

        var dialog = new ContentDialog
        {
            Title = "Weddenschappen Verwerkt",
            Content = "Alle weddenschappen zijn verwerkt op basis van de huidige resultaten.",
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };

        await dialog.ShowAsync();
    }
}
