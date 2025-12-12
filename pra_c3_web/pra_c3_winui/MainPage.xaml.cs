using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace pra_c3_winui;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        this.Loaded += MainPage_Loaded;
    }

    private void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Toon welkomsttekst
        if (App.CurrentPlayer != null)
        {
            WelcomeText.Text = $"Welkom, {App.CurrentPlayer.Name}!";

            if (App.CurrentPlayer.Admin)
            {
                AdminBadge.Visibility = Visibility.Visible;
            }
        }

        // Selecteer eerste item
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item)
        {
            var tag = item.Tag?.ToString();

            switch (tag)
            {
                case "Standings":
                    ContentFrame.Navigate(typeof(StandingsPage));
                    break;
                case "Teams":
                    ContentFrame.Navigate(typeof(TeamsPage));
                    break;
                case "Matches":
                    ContentFrame.Navigate(typeof(MatchesPage));
                    break;
            }
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        App.CurrentPlayer = null;

        if (App.MainWindow is MainWindow mainWindow)
        {
            mainWindow.NavigateTo(typeof(LoginPage));
        }
    }
}
