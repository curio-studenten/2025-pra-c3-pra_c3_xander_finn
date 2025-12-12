// =============================================================================
// MainWindow.xaml.cs - Hoofdvenster van de applicatie
// =============================================================================
// Dit is het hoofdvenster dat alle pagina's bevat.
// Het venster bevat een Frame element (ContentFrame) waarin pagina's worden
// geladen via navigatie. Dit zorgt voor een Single Page Application gevoel.
// =============================================================================

using Microsoft.UI.Xaml;  // WinUI framework voor Windows desktop apps

namespace pra_c3_winui;

/// <summary>
/// Het hoofdvenster van de Schoolvoetbal Gok applicatie.
/// 'sealed' betekent dat deze klasse niet kan worden overgeërfd.
/// 'partial' betekent dat de klasse ook gedefinieerd wordt in MainWindow.xaml.
/// </summary>
public sealed partial class MainWindow : Window
{
    // ===== Constructor =====

    /// <summary>
    /// Initialiseert het hoofdvenster en navigeert naar de login pagina.
    /// </summary>
    public MainWindow()
    {
        // Initialiseer de XAML componenten gedefinieerd in MainWindow.xaml
        // Dit laadt het ContentFrame element en andere UI elementen
        this.InitializeComponent();

        // Stel de titel van het venster in (zichtbaar in de titelbalk)
        this.Title = "Schoolvoetbal Gokken";

        // Navigeer naar de login pagina als startpagina
        // De gebruiker moet eerst inloggen voordat ze andere pagina's kunnen zien
        ContentFrame.Navigate(typeof(LoginPage));
    }

    // ===== Navigatie methodes =====

    /// <summary>
    /// Navigeert naar een opgegeven pagina type.
    /// Deze methode wordt gebruikt door andere pagina's om te navigeren.
    /// </summary>
    /// <param name="pageType">Het Type van de pagina om naar te navigeren (bijv. typeof(BettingPage)).</param>
    public void NavigateTo(Type pageType)
    {
        // Gebruik het Frame element om naar de nieuwe pagina te navigeren
        // Dit vervangt de huidige pagina door de nieuwe pagina
        ContentFrame.Navigate(pageType);
    }

    /// <summary>
    /// Navigeert specifiek naar de hoofdpagina (MainPage).
    /// Wordt aangeroepen na succesvol inloggen of registreren.
    /// </summary>
    public void NavigateToMainPage()
    {
        // Navigeer naar de MainPage waar de gebruiker kan kiezen wat te doen
        ContentFrame.Navigate(typeof(MainPage));
    }
}
