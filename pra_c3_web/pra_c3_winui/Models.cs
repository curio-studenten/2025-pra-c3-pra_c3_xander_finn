using System.Text.Json.Serialization;

namespace pra_c3_winui;

// ===== API Models (from fifa.amo.rocks) =====

public class ApiMatch
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("team1_id")]
    public int Team1Id { get; set; }

    [JsonPropertyName("team1_name")]
    public string Team1Name { get; set; } = string.Empty;

    [JsonPropertyName("team2_id")]
    public int Team2Id { get; set; }

    [JsonPropertyName("team2_name")]
    public string Team2Name { get; set; } = string.Empty;

    // Display helpers
    public string DisplayMatch => $"{Team1Name} vs {Team2Name}";
}

public class ApiResult
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("team1_id")]
    public int Team1Id { get; set; }

    [JsonPropertyName("team1_name")]
    public string Team1Name { get; set; } = string.Empty;

    [JsonPropertyName("team1_score")]
    public int Team1Score { get; set; }

    [JsonPropertyName("team2_id")]
    public int Team2Id { get; set; }

    [JsonPropertyName("team2_name")]
    public string Team2Name { get; set; } = string.Empty;

    [JsonPropertyName("team2_score")]
    public int Team2Score { get; set; }

    [JsonPropertyName("winner_id")]
    public int? WinnerId { get; set; }

    // Display helpers
    public string DisplayMatch => $"{Team1Name} vs {Team2Name}";
    public string DisplayScore => $"{Team1Score} - {Team2Score}";
    public string DisplayResult
    {
        get
        {
            if (WinnerId == Team1Id) return $"🏆 {Team1Name} wint";
            if (WinnerId == Team2Id) return $"🏆 {Team2Name} wint";
            return "🤝 Gelijkspel";
        }
    }
}

public class ApiGoal
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("match_id")]
    public int MatchId { get; set; }

    [JsonPropertyName("minute")]
    public int Minute { get; set; }

    [JsonPropertyName("player_id")]
    public int PlayerId { get; set; }

    [JsonPropertyName("player_team")]
    public int PlayerTeam { get; set; }

    [JsonPropertyName("player_name")]
    public string PlayerName { get; set; } = string.Empty;

    // Display helper
    public string DisplayGoal => $"⚽ {Minute}' - {PlayerName}";
}

// ===== Local User/Gambling Models =====

public class LocalUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public decimal Credits { get; set; } = 100m;
}

public class Bet
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int MatchId { get; set; }
    public string MatchDisplay { get; set; } = string.Empty;
    public BetType BetType { get; set; }
    public int? PredictedWinnerId { get; set; }
    public string PredictedWinnerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Odds { get; set; }
    public BetStatus Status { get; set; } = BetStatus.Pending;
    public decimal? Payout { get; set; }
    public DateTime PlacedAt { get; set; } = DateTime.Now;

    public string DisplayBet => BetType == BetType.Draw ? "Gelijkspel" : $"{PredictedWinnerName} wint";
    public string DisplayStatus => Status switch
    {
        BetStatus.Pending => "⏳ Lopend",
        BetStatus.Won => $"✅ Gewonnen (+€{Payout:F2})",
        BetStatus.Lost => "❌ Verloren",
        _ => "?"
    };
}

public enum BetType
{
    Team1Win,
    Team2Win,
    Draw
}

public enum BetStatus
{
    Pending,
    Won,
    Lost
}
