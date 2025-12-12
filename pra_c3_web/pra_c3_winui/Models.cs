
// 1. API Models - voor data die van de Laravel backend komt (wedstrijden, resultaten, doelpunten)
// 2. Local Models - voor lokale gebruikers- en weddenschapsgegevens

using System.Text.Json.Serialization;  // Nodig voor JSON serialisatie attributen

namespace pra_c3_winui;


/// Representeert een wedstrijd zoals ontvangen van de API.
/// Dit model wordt gebruikt om wedstrijden op te halen waarop nog gegokt kan worden.
/// De JsonPropertyName attributen zorgen ervoor dat de JSON velden correct worden gemapt.
public class ApiMatch
{
    /// <summary>
    /// Unieke identificatie van de wedstrijd in de database.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Database ID van het eerste team (thuisteam).
    /// </summary>
    [JsonPropertyName("team1_id")]
    public int Team1Id { get; set; }

    /// <summary>
    /// Naam van het eerste team (thuisteam), bijv. "Ajax" of "PSV".
    /// </summary>
    [JsonPropertyName("team1_name")]
    public string Team1Name { get; set; } = string.Empty;  // Lege string als default om null te voorkomen

    /// <summary>
    /// Database ID van het tweede team (uitteam).
    /// </summary>
    [JsonPropertyName("team2_id")]
    public int Team2Id { get; set; }

    /// <summary>
    /// Naam van het tweede team (uitteam), bijv. "Feyenoord" of "AZ".
    /// </summary>
    [JsonPropertyName("team2_name")]
    public string Team2Name { get; set; } = string.Empty;  // Lege string als default om null te voorkomen

    /// <summary>
    /// Helper property voor weergave in de UI.
    /// Geeft de wedstrijd weer als "Team1 vs Team2", bijv. "Ajax vs PSV".
    /// Deze property wordt gebruikt in de ListView bindings.
    /// </summary>
    public string DisplayMatch => $"{Team1Name} vs {Team2Name}";
}

/// <summary>
/// Representeert een wedstrijdresultaat van de API.
/// Dit model bevat de eindstand en winnaar informatie.
/// Wordt gebruikt om weddenschappen te verwerken en resultaten te tonen.
/// </summary>
public class ApiResult
{
    /// <summary>
    /// Unieke identificatie van de wedstrijd (zelfde als ApiMatch.Id).
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Database ID van het eerste team.
    /// </summary>
    [JsonPropertyName("team1_id")]
    public int Team1Id { get; set; }

    /// <summary>
    /// Naam van het eerste team.
    /// </summary>
    [JsonPropertyName("team1_name")]
    public string Team1Name { get; set; } = string.Empty;

    /// <summary>
    /// Eindscore van het eerste team (aantal doelpunten).
    /// </summary>
    [JsonPropertyName("team1_score")]
    public int Team1Score { get; set; }

    /// <summary>
    /// Database ID van het tweede team.
    /// </summary>
    [JsonPropertyName("team2_id")]
    public int Team2Id { get; set; }

    /// <summary>
    /// Naam van het tweede team.
    /// </summary>
    [JsonPropertyName("team2_name")]
    public string Team2Name { get; set; } = string.Empty;

    /// <summary>
    /// Eindscore van het tweede team (aantal doelpunten).
    /// </summary>
    [JsonPropertyName("team2_score")]
    public int Team2Score { get; set; }

    /// <summary>
    /// ID van het winnende team, of null bij gelijkspel.
    /// Dit veld is nullable omdat er geen winnaar is bij een gelijke stand.
    /// </summary>
    [JsonPropertyName("winner_id")]
    public int? WinnerId { get; set; }

    // ===== Display Helper Properties voor UI binding =====

    /// <summary>
    /// Weergave van de wedstrijd als "Team1 vs Team2".
    /// </summary>
    public string DisplayMatch => $"{Team1Name} vs {Team2Name}";

    /// <summary>
    /// Weergave van de score als "X - Y", bijv. "2 - 1".
    /// </summary>
    public string DisplayScore => $"{Team1Score} - {Team2Score}";

    /// <summary>
    /// Tekstuele weergave van het resultaat.
    /// Toont welk team gewonnen heeft of "Gelijkspel" bij gelijke stand.
    /// </summary>
    public string DisplayResult
    {
        get
        {
            // Controleer of Team 1 gewonnen heeft
            if (WinnerId == Team1Id) return $"{Team1Name} wint";
            // Controleer of Team 2 gewonnen heeft
            if (WinnerId == Team2Id) return $"{Team2Name} wint";
            // Anders is het gelijkspel (WinnerId is null)
            return "Gelijkspel";
        }
    }
}

/// <summary>
/// Representeert een doelpunt zoals ontvangen van de API.
/// Bevat informatie over wanneer en door wie het doelpunt is gemaakt.
/// </summary>
public class ApiGoal
{
    /// <summary>
    /// Unieke identificatie van het doelpunt in de database.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// ID van de wedstrijd waarin dit doelpunt is gemaakt.
    /// </summary>
    [JsonPropertyName("match_id")]
    public int MatchId { get; set; }

    /// <summary>
    /// De minuut waarin het doelpunt is gescoord (1-90+).
    /// </summary>
    [JsonPropertyName("minute")]
    public int Minute { get; set; }

    /// <summary>
    /// Database ID van de speler die scoorde.
    /// </summary>
    [JsonPropertyName("player_id")]
    public int PlayerId { get; set; }

    /// <summary>
    /// Team ID van de speler die scoorde (voor welk team het doelpunt telt).
    /// </summary>
    [JsonPropertyName("player_team")]
    public int PlayerTeam { get; set; }

    /// <summary>
    /// Naam van de speler die het doelpunt maakte.
    /// </summary>
    [JsonPropertyName("player_name")]
    public string PlayerName { get; set; } = string.Empty;

    /// <summary>
    /// Weergave van het doelpunt voor in de UI.
    /// Format: "minuut' - spelernaam", bijv. "45' - Jan Jansen".
    /// </summary>
    public string DisplayGoal => $"{Minute}' - {PlayerName}";
}

// =============================================================================
// LOKALE MODELS - Data structuren voor gebruikers en weddenschappen
// Deze data wordt lokaal opgeslagen in het geheugen van de applicatie.
// =============================================================================

/// <summary>
/// Representeert een lokale gebruiker van de gok applicatie.
/// Gebruikersgegevens worden in-memory opgeslagen (geen database).
/// </summary>
public class LocalUser
{
    /// <summary>
    /// Unieke identificatie van de gebruiker.
    /// Wordt automatisch gegenereerd bij registratie.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// De gebruikersnaam waarmee wordt ingelogd.
    /// Moet uniek zijn binnen de applicatie.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Het wachtwoord van de gebruiker.
    /// Let op: in een productie-omgeving zou dit gehashed moeten worden!
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Geeft aan of de gebruiker admin-rechten heeft.
    /// Admins kunnen weddenschappen verwerken en hebben extra functionaliteit.
    /// </summary>
    public bool IsAdmin { get; set; }

    /// <summary>
    /// Het huidige saldo van de gebruiker in euro's.
    /// Standaard begint elke nieuwe gebruiker met 100 euro.
    /// Dit saldo wordt gebruikt om weddenschappen te plaatsen en winsten uit te keren.
    /// </summary>
    public decimal Credits { get; set; } = 100m;  // 'm' suffix geeft aan dat het een decimal is
}

/// <summary>
/// Representeert een weddenschap geplaatst door een gebruiker.
/// Bevat alle informatie over de voorspelling, inzet, en uitkomst.
/// </summary>
public class Bet
{
    /// <summary>
    /// Unieke identificatie van de weddenschap.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID van de gebruiker die de weddenschap heeft geplaatst.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// ID van de wedstrijd waarop gegokt is.
    /// Wordt gebruikt om resultaten te koppelen aan weddenschappen.
    /// </summary>
    public int MatchId { get; set; }

    /// <summary>
    /// Weergavenaam van de wedstrijd (bijv. "Ajax vs PSV").
    /// Opgeslagen zodat dit niet elke keer opgehaald hoeft te worden.
    /// </summary>
    public string MatchDisplay { get; set; } = string.Empty;

    /// <summary>
    /// Het type weddenschap: Team1Win, Team2Win, of Draw.
    /// Bepaalt waarop de gebruiker heeft gegokt.
    /// </summary>
    public BetType BetType { get; set; }

    /// <summary>
    /// ID van het team waarvan de gebruiker voorspelt dat het wint.
    /// Is null bij een gelijkspel-weddenschap.
    /// </summary>
    public int? PredictedWinnerId { get; set; }

    /// <summary>
    /// Naam van het voorspelde winnende team voor weergave.
    /// </summary>
    public string PredictedWinnerName { get; set; } = string.Empty;

    /// <summary>
    /// Het bedrag dat is ingezet in euro's.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// De odds (quoteringen) op moment van inzetten.
    /// Team winst heeft odds van 2.0, gelijkspel heeft odds van 3.5.
    /// Bij winst: uitbetaling = inzet * odds.
    /// </summary>
    public decimal Odds { get; set; }

    /// <summary>
    /// De huidige status van de weddenschap.
    /// Pending = nog niet afgelopen, Won = gewonnen, Lost = verloren.
    /// </summary>
    public BetStatus Status { get; set; } = BetStatus.Pending;  // Standaard is lopend

    /// <summary>
    /// Het uitbetaalde bedrag bij winst, null als nog niet afgelopen of verloren.
    /// Berekening: Amount * Odds.
    /// </summary>
    public decimal? Payout { get; set; }

    /// <summary>
    /// Tijdstip waarop de weddenschap is geplaatst.
    /// Standaard het huidige moment.
    /// </summary>
    public DateTime PlacedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Helper property voor weergave van de voorspelling.
    /// Toont "Gelijkspel" of "[Teamnaam] wint".
    /// </summary>
    public string DisplayBet => BetType == BetType.Draw ? "Gelijkspel" : $"{PredictedWinnerName} wint";

    /// <summary>
    /// Helper property voor weergave van de status.
    /// Toont Nederlandse tekst: "Lopend", "Gewonnen (+bedrag)", of "Verloren".
    /// Gebruikt een switch expression voor compacte code.
    /// </summary>
    public string DisplayStatus => Status switch
    {
        BetStatus.Pending => "Lopend",                    // Wedstrijd nog niet afgelopen
        BetStatus.Won => $"Gewonnen (+{Payout:F2})",     // Gewonnen met uitbetaald bedrag
        BetStatus.Lost => "Verloren",                    // Verloren
        _ => "?"                                          // Fallback voor onbekende status
    };
}

// =============================================================================
// ENUMERATIES - Voorgedefinieerde waardes voor type-veiligheid
// =============================================================================

/// <summary>
/// Definieert de mogelijke types weddenschappen.
/// </summary>
public enum BetType
{
    /// <summary>
    /// Weddenschap dat Team 1 (thuisteam) wint.
    /// </summary>
    Team1Win,

    /// <summary>
    /// Weddenschap dat Team 2 (uitteam) wint.
    /// </summary>
    Team2Win,

    /// <summary>
    /// Weddenschap op gelijkspel (geen winnaar).
    /// </summary>
    Draw
}

/// <summary>
/// Definieert de mogelijke statussen van een weddenschap.
/// </summary>
public enum BetStatus
{
    /// <summary>
    /// De wedstrijd is nog niet afgelopen, resultaat onbekend.
    /// </summary>
    Pending,

    /// <summary>
    /// De voorspelling was correct, gebruiker heeft gewonnen.
    /// </summary>
    Won,

    /// <summary>
    /// De voorspelling was fout, gebruiker heeft verloren.
    /// </summary>
    Lost
}
