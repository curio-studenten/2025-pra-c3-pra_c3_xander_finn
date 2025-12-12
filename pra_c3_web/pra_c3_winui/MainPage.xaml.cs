using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace pra_c3_winui;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        UpdateUserInfo();

        // Select first item
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    private void UpdateUserInfo()
    {
        var user = App.DataService.CurrentUser;
        if (user != null)
        {
            UserInfoText.Text = user.IsAdmin ? $"👑 Admin: {user.Username}" : $"🎰 Gokker: {user.Username}";
            CreditsText.Text = user.IsAdmin ? "" : $"💰 €{user.Credits:F2}";
        }
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item)
        {
            var tag = item.Tag?.ToString();
            switch (tag)
            {
                case "betting":
                    ContentFrame.Navigate(typeof(BettingPage));
                    break;
                case "mybets":
                    ContentFrame.Navigate(typeof(MyBetsPage));
                    break;
                case "results":
                    ContentFrame.Navigate(typeof(ResultsPage));
                    break;
            }
        }
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        App.DataService.Logout();
        if (App.MainWindow is MainWindow mainWindow)
        {
            mainWindow.NavigateTo(typeof(LoginPage));
        }
    }

    public void RefreshCredits()
    {
        UpdateUserInfo();
    }
}
