using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace pra_c3_winui;

public class TeamWithPosition
{
    public int Position { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Points { get; set; }
}

public sealed partial class StandingsPage : Page
{
    public StandingsPage()
    {
        this.InitializeComponent();
        this.Loaded += StandingsPage_Loaded;
    }

    private async void StandingsPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadStandingsAsync();
    }

    private async Task LoadStandingsAsync()
    {
        LoadingRing.IsActive = true;
        RefreshButton.IsEnabled = false;

        try
        {
            var standings = await MainWindow.ApiService.GetStandingsAsync();

            if (standings.Count > 0)
            {
                var teamsWithPosition = standings.Select((team, index) => new TeamWithPosition
                {
                    Position = index + 1,
                    Name = team.Name,
                    Points = team.Points
                }).ToList();

                StandingsListView.ItemsSource = teamsWithPosition;
                StandingsListView.Visibility = Visibility.Visible;
                EmptyState.Visibility = Visibility.Collapsed;
            }
            else
            {
                StandingsListView.Visibility = Visibility.Collapsed;
                EmptyState.Visibility = Visibility.Visible;
            }
        }
        catch
        {
            StandingsListView.Visibility = Visibility.Collapsed;
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
        await LoadStandingsAsync();
    }
}
