using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace pra_c3_winui;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://fifa.amo.rocks";

    public ApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// GET /api/matches.php - Alle wedstrijden (nog niet gespeeld)
    /// </summary>
    public async Task<List<ApiMatch>> GetMatchesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/matches.php");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseBody))
            {
                return JsonSerializer.Deserialize<List<ApiMatch>>(responseBody) ?? new List<ApiMatch>();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching matches: {ex.Message}");
        }

        return new List<ApiMatch>();
    }

    /// <summary>
    /// GET /api/results.php - Alle resultaten (gespeelde wedstrijden)
    /// </summary>
    public async Task<List<ApiResult>> GetResultsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/results.php");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseBody))
            {
                return JsonSerializer.Deserialize<List<ApiResult>>(responseBody) ?? new List<ApiResult>();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching results: {ex.Message}");
        }

        return new List<ApiResult>();
    }

    /// <summary>
    /// GET /api/goals.php?match_id={id} - Goals van een specifieke wedstrijd
    /// </summary>
    public async Task<List<ApiGoal>> GetGoalsAsync(int matchId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/api/goals.php?match_id={matchId}");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseBody))
            {
                return JsonSerializer.Deserialize<List<ApiGoal>>(responseBody) ?? new List<ApiGoal>();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching goals: {ex.Message}");
        }

        return new List<ApiGoal>();
    }
}

/// <summary>
/// Local storage service for users and bets (stored in memory/local file)
/// </summary>
public class LocalDataService
{
    private List<LocalUser> _users = new();
    private List<Bet> _bets = new();
    private int _nextUserId = 1;
    private int _nextBetId = 1;

    public LocalUser? CurrentUser { get; private set; }

    public LocalDataService()
    {
        // Default admin account
        _users.Add(new LocalUser
        {
            Id = _nextUserId++,
            Username = "admin",
            Password = "admin123",
            IsAdmin = true,
            Credits = 0
        });

        // Default gambler accounts
        _users.Add(new LocalUser
        {
            Id = _nextUserId++,
            Username = "gokker1",
            Password = "wachtwoord",
            IsAdmin = false,
            Credits = 100m
        });

        _users.Add(new LocalUser
        {
            Id = _nextUserId++,
            Username = "gokker2",
            Password = "wachtwoord",
            IsAdmin = false,
            Credits = 100m
        });
    }

    public bool Login(string username, string password)
    {
        var user = _users.FirstOrDefault(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            u.Password == password);

        if (user != null)
        {
            CurrentUser = user;
            return true;
        }
        return false;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public bool Register(string username, string password)
    {
        if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
        {
            return false; // Username already exists
        }

        _users.Add(new LocalUser
        {
            Id = _nextUserId++,
            Username = username,
            Password = password,
            IsAdmin = false,
            Credits = 100m
        });

        return true;
    }

    public bool PlaceBet(int matchId, string matchDisplay, BetType betType, int? predictedWinnerId, string predictedWinnerName, decimal amount, decimal odds)
    {
        if (CurrentUser == null || CurrentUser.IsAdmin) return false;
        if (CurrentUser.Credits < amount) return false;

        // Check if already bet on this match
        if (_bets.Any(b => b.UserId == CurrentUser.Id && b.MatchId == matchId))
        {
            return false;
        }

        CurrentUser.Credits -= amount;

        _bets.Add(new Bet
        {
            Id = _nextBetId++,
            UserId = CurrentUser.Id,
            MatchId = matchId,
            MatchDisplay = matchDisplay,
            BetType = betType,
            PredictedWinnerId = predictedWinnerId,
            PredictedWinnerName = predictedWinnerName,
            Amount = amount,
            Odds = odds,
            Status = BetStatus.Pending,
            PlacedAt = DateTime.Now
        });

        return true;
    }

    public List<Bet> GetUserBets()
    {
        if (CurrentUser == null) return new List<Bet>();
        return _bets.Where(b => b.UserId == CurrentUser.Id).OrderByDescending(b => b.PlacedAt).ToList();
    }

    public List<Bet> GetPendingBets()
    {
        return _bets.Where(b => b.Status == BetStatus.Pending).ToList();
    }

    public void ProcessBetResult(int matchId, int? winnerId)
    {
        var matchBets = _bets.Where(b => b.MatchId == matchId && b.Status == BetStatus.Pending).ToList();

        foreach (var bet in matchBets)
        {
            bool won = false;

            if (bet.BetType == BetType.Draw && winnerId == null)
            {
                won = true;
            }
            else if (bet.BetType != BetType.Draw && bet.PredictedWinnerId == winnerId)
            {
                won = true;
            }

            if (won)
            {
                bet.Status = BetStatus.Won;
                bet.Payout = bet.Amount * bet.Odds;

                var user = _users.FirstOrDefault(u => u.Id == bet.UserId);
                if (user != null)
                {
                    user.Credits += bet.Payout.Value;
                }
            }
            else
            {
                bet.Status = BetStatus.Lost;
                bet.Payout = 0;
            }
        }
    }

    public bool HasBetOnMatch(int matchId)
    {
        if (CurrentUser == null) return false;
        return _bets.Any(b => b.UserId == CurrentUser.Id && b.MatchId == matchId);
    }
}
