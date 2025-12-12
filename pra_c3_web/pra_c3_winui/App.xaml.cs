// =============================================================================
// App.xaml.cs - Hoofdapplicatie klasse voor de WinUI 3 app
// =============================================================================
// Dit is het startpunt van de WinUI 3 applicatie.
// Hier worden de globale services geïnitialiseerd die door alle pagina's
// worden gebruikt (ApiService en DataService).
// =============================================================================

using Microsoft.UI.Xaml;  // WinUI framework voor Windows desktop apps

namespace pra_c3_winui;

/// <summary>
/// De hoofdapplicatie klasse die de WinUI 3 app beheert.
/// 'partial' betekent dat de klasse ook gedefinieerd wordt in App.xaml (XAML deel).
/// Erft van Application, de basis WinUI applicatie klasse.
/// </summary>
public partial class App : Application
{
    // ===== Statische properties - Toegankelijk vanuit alle pagina's =====

    /// <summary>
    /// Referentie naar het hoofdvenster van de applicatie.
    /// Static zodat elke pagina toegang heeft tot het venster.
    /// Nullable (?) omdat het null is voordat OnLaunched wordt aangeroepen.
    /// </summary>
    public static Window? MainWindow { get; private set; }

    /// <summary>
    /// De ApiService voor communicatie met de Laravel backend.
    /// Static en readonly - één instantie voor de hele applicatie.
    /// Wordt gebruikt door alle pagina's om data op te halen.
    /// </summary>
    public static ApiService ApiService { get; } = new ApiService();

    /// <summary>
    /// De LocalDataService voor gebruikers- en weddenschapsbeheer.
    /// Static en readonly - één instantie voor de hele applicatie.
    /// Bevat de ingelogde gebruiker en alle weddenschappen.
    /// </summary>
    public static LocalDataService DataService { get; } = new LocalDataService();

    // ===== Constructor =====

    /// <summary>
    /// Constructor - wordt aangeroepen bij het starten van de applicatie.
    /// </summary>
    public App()
    {
        // Initialiseer de XAML componenten gedefinieerd in App.xaml
        // Dit laadt resources, styles, en andere XAML configuratie
        this.InitializeComponent();
    }

    // ===== Lifecycle methodes =====

    /// <summary>
    /// Wordt aangeroepen wanneer de applicatie wordt gestart.
    /// Dit is het startpunt waar het hoofdvenster wordt aangemaakt en getoond.
    /// </summary>
    /// <param name="args">Bevat informatie over hoe de app is gestart.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Maak het hoofdvenster aan
        MainWindow = new MainWindow();

        // Activeer (toon) het venster
        // Dit maakt het venster zichtbaar voor de gebruiker
        MainWindow.Activate();
    }
}
