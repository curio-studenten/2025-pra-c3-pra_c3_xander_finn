using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace pra_c3_winui;

public sealed partial class TeamsPage : Page
{
    public TeamsPage()
    {
        this.InitializeComponent();
        this.Loaded += TeamsPage_Loaded;
    }

    private async void TeamsPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadTeamsAsync();
    }

    private async Task LoadTeamsAsync()
    {
        LoadingRing.IsActive = true;
        RefreshButton.IsEnabled = false;

        try
        {
            var teams = await MainWindow.ApiService.GetTeamsAsync();

            if (teams.Count > 0)
            {
                TeamsGridView.ItemsSource = teams;
                TeamsGridView.Visibility = Visibility.Visible;
                EmptyState.Visibility = Visibility.Collapsed;
            }
            else
            {
                TeamsGridView.Visibility = Visibility.Collapsed;
                EmptyState.Visibility = Visibility.Visible;
            }
        }
        catch
        {
            TeamsGridView.Visibility = Visibility.Collapsed;
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
        await LoadTeamsAsync();
    }
}
