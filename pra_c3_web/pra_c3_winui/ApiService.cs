using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace pra_c3_winui;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private string? _apiKey;

    public ApiService(string baseUrl = "http://localhost:8000")
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public void SetApiKey(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient.DefaultRequestHeaders.Remove("X-API-KEY");
        _httpClient.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
    }

    public async Task<RegisterResponse?> RegisterAsync(string name, string email, string password, string role = "player")
    {
        try
        {
            var data = new { name, email, password, role };
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/register", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<RegisterResponse>(responseBody);
            }
            else
            {
                // Try to parse error response
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<RegisterResponse>(responseBody);
                    return errorResponse;
                }
                catch
                {
                    return new RegisterResponse { Success = false, Message = $"Error: {response.StatusCode}" };
                }
            }
        }
        catch (Exception ex)
        {
            return new RegisterResponse { Success = false, Message = $"Connection error: {ex.Message}" };
        }
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        try
        {
            var data = new { email, password };
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/login", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody);

            if (loginResponse?.Success == true && loginResponse.Player != null)
            {
                SetApiKey(loginResponse.Player.ApiKey);
            }

            return loginResponse;
        }
        catch (Exception ex)
        {
            return new LoginResponse { Success = false, Error = $"Connection error: {ex.Message}" };
        }
    }

    public async Task<List<Team>> GetTeamsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/teams");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<List<Team>>(responseBody) ?? new List<Team>();
            }
        }
        catch
        {
            // Handle error
        }

        return new List<Team>();
    }

    public async Task<List<Match>> GetMatchesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/matches");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<List<Match>>(responseBody) ?? new List<Match>();
            }
        }
        catch
        {
            // Handle error
        }

        return new List<Match>();
    }

    public async Task<List<Team>> GetStandingsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/standings");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<List<Team>>(responseBody) ?? new List<Team>();
            }
        }
        catch
        {
            // Handle error
        }

        return new List<Team>();
    }

    public async Task<bool> UpdateMatchScoreAsync(int matchId, int scoreTeam1, int scoreTeam2)
    {
        try
        {
            var data = new { score_team1 = scoreTeam1, score_team2 = scoreTeam2 };
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_baseUrl}/api/matches/{matchId}", content);

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
