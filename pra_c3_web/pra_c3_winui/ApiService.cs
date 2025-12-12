// =============================================================================
// ApiService.cs - Services voor API communicatie en lokale data opslag
// =============================================================================
// Dit bestand bevat twee belangrijke services:
// 1. ApiService - Communiceert met de Laravel backend API via HTTP
// 2. LocalDataService - Beheert lokale gebruikers en weddenschappen in-memory
// =============================================================================

using System.Net.Http;           // Voor HttpClient om HTTP requests te maken
using System.Net.Http.Headers;   // Voor het instellen van HTTP headers
using System.Text.Json;          // Voor JSON serialisatie/deserialisatie

namespace pra_c3_winui;

// =============================================================================
// API SERVICE - Haalt data op van de Laravel backend server
// =============================================================================

/// <summary>
/// Service klasse voor alle communicatie met de Laravel API.
/// Gebruikt HttpClient om HTTP GET requests te maken naar de backend.
/// Alle methodes zijn async om de UI niet te blokkeren tijdens netwerk calls.
/// </summary>
public class ApiService
{
    // ===== Private velden =====

    /// <summary>
    /// De HttpClient instantie voor het maken van HTTP requests.
    /// Is readonly omdat we dezelfde client hergebruiken voor alle requests.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// De basis URL van de Laravel API server.
    /// Dit is de lokale development server die draait via 'php artisan serve'.
    /// </summary>
    private const string BaseUrl = "http://127.0.0.1:8000";

    // ===== Constructor =====

    /// <summary>
    /// Initialiseert de ApiService met een geconfigureerde HttpClient.
    /// </summary>
    public ApiService()
    {
        // Maak een nieuwe HttpClient instantie
        _httpClient = new HttpClient();

        // Stel in dat we JSON responses verwachten
        // Dit voegt de header "Accept: application/json" toe aan alle requests
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    // ===== API Methodes =====

    /// <summary>
    /// Haalt alle beschikbare wedstrijden op van de API.
    /// Dit zijn wedstrijden die nog niet gespeeld zijn en waarop gegokt kan worden.
    /// Endpoint: GET /api/matches.php
    /// </summary>
    /// <returns>Een lijst van ApiMatch objecten, of een lege lijst bij fouten.</returns>
    public async Task<List<ApiMatch>> GetMatchesAsync()
    {
        try
        {
            // Maak een GET request naar het matches endpoint
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/matches.php");

            // Lees de response body als string
            var responseBody = await response.Content.ReadAsStringAsync();

            // Controleer of de request succesvol was en er data is
            if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseBody))
            {
                // Deserialiseer de JSON naar een lijst van ApiMatch objecten
                // De ?? operator geeft een lege lijst terug als deserialisatie null oplevert
                return JsonSerializer.Deserialize<List<ApiMatch>>(responseBody) ?? new List<ApiMatch>();
            }
        }
        catch (Exception ex)
        {
            // Log de fout naar de debug output (zichtbaar in Visual Studio)
            // Dit helpt bij het debuggen van netwerk problemen
            System.Diagnostics.Debug.WriteLine($"Error fetching matches: {ex.Message}");
        }

        // Bij fouten of lege response, geef een lege lijst terug
        return new List<ApiMatch>();
    }

    /// <summary>
    /// Haalt alle wedstrijdresultaten op van de API.
    /// Dit zijn wedstrijden die al gespeeld zijn met scores en winnaars.
    /// Endpoint: GET /api/results.php
    /// </summary>
    /// <returns>Een lijst van ApiResult objecten, of een lege lijst bij fouten.</returns>
    public async Task<List<ApiResult>> GetResultsAsync()
    {
        try
        {
            // Maak een GET request naar het results endpoint
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/results.php");

            // Lees de response body als string
            var responseBody = await response.Content.ReadAsStringAsync();

            // Controleer of de request succesvol was en er data is
            if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseBody))
            {
                // Deserialiseer de JSON naar een lijst van ApiResult objecten
                return JsonSerializer.Deserialize<List<ApiResult>>(responseBody) ?? new List<ApiResult>();
            }
        }
        catch (Exception ex)
        {
            // Log de fout voor debugging doeleinden
            System.Diagnostics.Debug.WriteLine($"Error fetching results: {ex.Message}");
        }

        // Bij fouten, geef een lege lijst terug
        return new List<ApiResult>();
    }

    /// <summary>
    /// Haalt alle doelpunten op voor een specifieke wedstrijd.
    /// Endpoint: GET /api/goals.php?match_id={matchId}
    /// </summary>
    /// <param name="matchId">Het ID van de wedstrijd waarvan de doelpunten worden opgehaald.</param>
    /// <returns>Een lijst van ApiGoal objecten, of een lege lijst bij fouten of geen doelpunten.</returns>
    public async Task<List<ApiGoal>> GetGoalsAsync(int matchId)
    {
        try
        {
            // Maak een GET request met het match_id als query parameter
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/goals.php?match_id={matchId}");

            // Lees de response body als string
            var responseBody = await response.Content.ReadAsStringAsync();

            // Controleer of de request succesvol was en er data is
            if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseBody))
            {
                // Deserialiseer de JSON naar een lijst van ApiGoal objecten
                return JsonSerializer.Deserialize<List<ApiGoal>>(responseBody) ?? new List<ApiGoal>();
            }
        }
        catch (Exception ex)
        {
            // Log de fout voor debugging doeleinden
            System.Diagnostics.Debug.WriteLine($"Error fetching goals: {ex.Message}");
        }

        // Bij fouten, geef een lege lijst terug
        return new List<ApiGoal>();
    }
}

// =============================================================================
// LOCAL DATA SERVICE - Beheert gebruikers en weddenschappen in-memory
// =============================================================================

/// <summary>
/// Service klasse voor lokale data opslag van gebruikers en weddenschappen.
/// Alle data wordt in-memory opgeslagen (gaat verloren bij app herstart).
/// In een productie-omgeving zou dit vervangen worden door een database.
/// </summary>
public class LocalDataService
{
    // ===== Private velden voor data opslag =====

    /// <summary>
    /// Lijst van alle geregistreerde gebruikers.
    /// Bevat zowel admins als gewone gokkers.
    /// </summary>
    private List<LocalUser> _users = new();

    /// <summary>
    /// Lijst van alle geplaatste weddenschappen.
    /// Bevat weddenschappen van alle gebruikers.
    /// </summary>
    private List<Bet> _bets = new();

    /// <summary>
    /// Teller voor het genereren van unieke gebruikers-IDs.
    /// Wordt verhoogd bij elke nieuwe registratie.
    /// </summary>
    private int _nextUserId = 1;

    /// <summary>
    /// Teller voor het genereren van unieke weddenschap-IDs.
    /// Wordt verhoogd bij elke nieuwe weddenschap.
    /// </summary>
    private int _nextBetId = 1;

    // ===== Public properties =====

    /// <summary>
    /// De momenteel ingelogde gebruiker.
    /// Is null als niemand is ingelogd.
    /// Wordt gezet door Login() en gewist door Logout().
    /// </summary>
    public LocalUser? CurrentUser { get; private set; }

    // ===== Constructor =====

    /// <summary>
    /// Initialiseert de LocalDataService met standaard test accounts.
    /// Dit zorgt ervoor dat er altijd accounts beschikbaar zijn voor testen.
    /// </summary>
    public LocalDataService()
    {
        // ===== Standaard admin account =====
        // Dit account heeft beheerdersrechten en kan weddenschappen verwerken
        _users.Add(new LocalUser
        {
            Id = _nextUserId++,          // ID = 1, verhoog teller naar 2
            Username = "admin",           // Gebruikersnaam voor inloggen
            Password = "admin123",        // Wachtwoord (in productie zou dit gehashed zijn)
            IsAdmin = true,               // Admin heeft speciale rechten
            Credits = 0                   // Admin gokt niet, dus 0 credits
        });

        // ===== Standaard gokker account 1 =====
        // Een test account voor het testen van gok functionaliteit
        _users.Add(new LocalUser
        {
            Id = _nextUserId++,          // ID = 2, verhoog teller naar 3
            Username = "gokker1",         // Gebruikersnaam
            Password = "wachtwoord",      // Wachtwoord
            IsAdmin = false,              // Geen admin rechten
            Credits = 100m                // Start met 100 euro (m = decimal literal)
        });

        // ===== Standaard gokker account 2 =====
        // Nog een test account voor het testen met meerdere gebruikers
        _users.Add(new LocalUser
        {
            Id = _nextUserId++,          // ID = 3, verhoog teller naar 4
            Username = "gokker2",         // Gebruikersnaam
            Password = "wachtwoord",      // Wachtwoord
            IsAdmin = false,              // Geen admin rechten
            Credits = 100m                // Start met 100 euro
        });
    }

    // ===== Authenticatie methodes =====

    /// <summary>
    /// Probeert een gebruiker in te loggen met gebruikersnaam en wachtwoord.
    /// </summary>
    /// <param name="username">De ingevoerde gebruikersnaam.</param>
    /// <param name="password">Het ingevoerde wachtwoord.</param>
    /// <returns>True als login succesvol is, anders False.</returns>
    public bool Login(string username, string password)
    {
        // Zoek een gebruiker met matching gebruikersnaam (case-insensitive) en exact wachtwoord
        var user = _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&  // Gebruikersnaam is niet hoofdlettergevoelig
            u.Password == password);  // Wachtwoord moet exact matchen

        // Als een gebruiker gevonden is
        if (user != null)
        {
            // Sla de ingelogde gebruiker op
            CurrentUser = user;
            return true;  // Login succesvol
        }

        return false;  // Login mislukt - verkeerde credentials
    }

    /// <summary>
    /// Logt de huidige gebruiker uit.
    /// </summary>
    public void Logout()
    {
        // Wis de huidige gebruiker
        CurrentUser = null;
    }

    /// <summary>
    /// Registreert een nieuwe gebruiker met de gegeven credentials.
    /// </summary>
    /// <param name="username">De gewenste gebruikersnaam.</param>
    /// <param name="password">Het gewenste wachtwoord.</param>
    /// <returns>True als registratie succesvol is, False als gebruikersnaam al bestaat.</returns>
    public bool Register(string username, string password)
    {
        // Controleer of de gebruikersnaam al in gebruik is (case-insensitive)
        if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
        {
            return false;  // Gebruikersnaam bestaat al
        }

        // Maak een nieuwe gebruiker aan
        _users.Add(new LocalUser
        {
            Id = _nextUserId++,          // Wijs een uniek ID toe en verhoog de teller
            Username = username,          // De gekozen gebruikersnaam
            Password = password,          // Het gekozen wachtwoord
            IsAdmin = false,              // Nieuwe gebruikers zijn nooit admin
            Credits = 100m                // Iedereen start met 100 euro
        });

        return true;  // Registratie succesvol
    }

    // ===== Weddenschap methodes =====

    /// <summary>
    /// Plaatst een nieuwe weddenschap voor de huidige gebruiker.
    /// Controleert of de gebruiker genoeg credits heeft en nog niet op deze wedstrijd heeft gegokt.
    /// </summary>
    /// <param name="matchId">ID van de wedstrijd.</param>
    /// <param name="matchDisplay">Weergavenaam van de wedstrijd (bijv. "Ajax vs PSV").</param>
    /// <param name="betType">Type weddenschap (Team1Win, Team2Win, of Draw).</param>
    /// <param name="predictedWinnerId">ID van het voorspelde winnende team (null bij Draw).</param>
    /// <param name="predictedWinnerName">Naam van het voorspelde winnende team.</param>
    /// <param name="amount">Het in te zetten bedrag.</param>
    /// <param name="odds">De quoteringen voor deze weddenschap.</param>
    /// <returns>True als de weddenschap succesvol is geplaatst, anders False.</returns>
    public bool PlaceBet(int matchId, string matchDisplay, BetType betType, int? predictedWinnerId, string predictedWinnerName, decimal amount, decimal odds)
    {
        // Controleer of er een gebruiker is ingelogd en of het geen admin is
        // Admins mogen niet gokken
        if (CurrentUser == null || CurrentUser.IsAdmin) return false;

        // Controleer of de gebruiker genoeg credits heeft
        if (CurrentUser.Credits < amount) return false;

        // Controleer of de gebruiker al op deze wedstrijd heeft gegokt
        // Elke gebruiker mag maar één weddenschap per wedstrijd plaatsen
        if (_bets.Any(b => b.UserId == CurrentUser.Id && b.MatchId == matchId))
        {
            return false;  // Al gegokt op deze wedstrijd
        }

        // Trek het ingezette bedrag af van de credits
        CurrentUser.Credits -= amount;

        // Maak de nieuwe weddenschap aan en voeg toe aan de lijst
        _bets.Add(new Bet
        {
            Id = _nextBetId++,                    // Uniek ID voor deze weddenschap
            UserId = CurrentUser.Id,              // Koppel aan de huidige gebruiker
            MatchId = matchId,                    // Koppel aan de wedstrijd
            MatchDisplay = matchDisplay,          // Bewaar de weergavenaam
            BetType = betType,                    // Type voorspelling
            PredictedWinnerId = predictedWinnerId,// Voorspelde winnaar (null bij gelijkspel)
            PredictedWinnerName = predictedWinnerName,  // Naam van voorspelde winnaar
            Amount = amount,                      // Ingezet bedrag
            Odds = odds,                          // Quoteringen op moment van inzet
            Status = BetStatus.Pending,           // Begint als lopend
            PlacedAt = DateTime.Now               // Tijdstip van plaatsing
        });

        return true;  // Weddenschap succesvol geplaatst
    }

    /// <summary>
    /// Haalt alle weddenschappen van de huidige gebruiker op.
    /// Gesorteerd van nieuwste naar oudste.
    /// </summary>
    /// <returns>Lijst van weddenschappen van de huidige gebruiker.</returns>
    public List<Bet> GetUserBets()
    {
        // Als er geen gebruiker is ingelogd, geef lege lijst
        if (CurrentUser == null) return new List<Bet>();

        // Filter weddenschappen op gebruiker en sorteer op datum (nieuwste eerst)
        return _bets.Where(b => b.UserId == CurrentUser.Id)
                    .OrderByDescending(b => b.PlacedAt)
                    .ToList();
    }

    /// <summary>
    /// Haalt alle nog lopende weddenschappen op (alle gebruikers).
    /// Wordt gebruikt door admins om te zien welke weddenschappen nog verwerkt moeten worden.
    /// </summary>
    /// <returns>Lijst van alle lopende weddenschappen.</returns>
    public List<Bet> GetPendingBets()
    {
        // Filter op status = Pending
        return _bets.Where(b => b.Status == BetStatus.Pending).ToList();
    }

    /// <summary>
    /// Verwerkt het resultaat van een wedstrijd voor alle bijbehorende weddenschappen.
    /// Controleert of voorspellingen correct waren en kent winsten toe.
    /// </summary>
    /// <param name="matchId">ID van de afgelopen wedstrijd.</param>
    /// <param name="winnerId">ID van het winnende team, of null bij gelijkspel.</param>
    public void ProcessBetResult(int matchId, int? winnerId)
    {
        // Vind alle lopende weddenschappen voor deze wedstrijd
        var matchBets = _bets.Where(b => b.MatchId == matchId && b.Status == BetStatus.Pending).ToList();

        // Verwerk elke weddenschap
        foreach (var bet in matchBets)
        {
            bool won = false;  // Standaard: niet gewonnen

            // Controleer of de voorspelling correct was
            if (bet.BetType == BetType.Draw && winnerId == null)
            {
                // Voorspelling was gelijkspel en het was ook gelijkspel
                won = true;
            }
            else if (bet.BetType != BetType.Draw && bet.PredictedWinnerId == winnerId)
            {
                // Voorspelling was een team winst en dat team heeft ook gewonnen
                won = true;
            }

            if (won)
            {
                // De voorspelling was correct!
                bet.Status = BetStatus.Won;

                // Bereken de uitbetaling: inzet * odds
                bet.Payout = bet.Amount * bet.Odds;

                // Vind de gebruiker en schrijf de winst bij op hun saldo
                var user = _users.FirstOrDefault(u => u.Id == bet.UserId);
                if (user != null)
                {
                    user.Credits += bet.Payout.Value;  // .Value omdat Payout nullable is
                }
            }
            else
            {
                // De voorspelling was fout
                bet.Status = BetStatus.Lost;
                bet.Payout = 0;  // Geen uitbetaling
                // Het ingezette bedrag was al afgetrokken bij het plaatsen van de weddenschap
            }
        }
    }

    /// <summary>
    /// Controleert of de huidige gebruiker al op een bepaalde wedstrijd heeft gegokt.
    /// Wordt gebruikt om te voorkomen dat gebruikers meerdere keren op dezelfde wedstrijd gokken.
    /// </summary>
    /// <param name="matchId">ID van de wedstrijd om te controleren.</param>
    /// <returns>True als de gebruiker al een weddenschap heeft op deze wedstrijd.</returns>
    public bool HasBetOnMatch(int matchId)
    {
        // Als er geen gebruiker is ingelogd, return false
        if (CurrentUser == null) return false;

        // Controleer of er een weddenschap bestaat van deze gebruiker op deze wedstrijd
        return _bets.Any(b => b.UserId == CurrentUser.Id && b.MatchId == matchId);
    }
}
