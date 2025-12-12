using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace pra_c3_winui;

public sealed partial class MyBetsPage : Page
{
    public MyBetsPage()
    {
        this.InitializeComponent();
        LoadBets();
    }

    private void LoadBets()
    {
        var bets = App.DataService.GetUserBets();
        BetsListView.ItemsSource = bets;

        if (bets.Count == 0)
        {
            NoBetsText.Visibility = Visibility.Visible;
            BetsListView.Visibility = Visibility.Collapsed;
        }
        else
        {
            NoBetsText.Visibility = Visibility.Collapsed;
            BetsListView.Visibility = Visibility.Visible;
        }
    }
}
