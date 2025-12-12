using System.Text.Json.Serialization;

namespace pra_c3_winui;

public class Player
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("admin")]
    public bool Admin { get; set; }

    [JsonPropertyName("api_key")]
    public string ApiKey { get; set; } = string.Empty;
}

public class Team
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("points")]
    public int Points { get; set; }

    [JsonPropertyName("creator_id")]
    public int CreatorId { get; set; }
}

public class Match
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

    [JsonPropertyName("score_team1")]
    public int? ScoreTeam1 { get; set; }

    [JsonPropertyName("score_team2")]
    public int? ScoreTeam2 { get; set; }

    [JsonPropertyName("field")]
    public int Field { get; set; }

    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    [JsonPropertyName("played")]
    public bool Played { get; set; }

    // Display property voor de UI
    public string DisplayScore => Played ? $"{ScoreTeam1} - {ScoreTeam2}" : "- : -";
    public string DisplayMatch => $"{Team1Name} vs {Team2Name}";
    public string DisplayStatus => Played ? "Gespeeld" : "Gepland";
}

public class ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;

    [JsonPropertyName("player")]
    public T? Data { get; set; }
}

public class LoginResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;

    [JsonPropertyName("player")]
    public Player? Player { get; set; }
}

public class RegisterResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("player")]
    public Player? Player { get; set; }
}
