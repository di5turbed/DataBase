using System.Net.Http.Json;
using System.Windows.Forms;

namespace DesktopClient
{
    public class TeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int PlayersCount { get; set; }
    }
    public class PlayerDto
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class TournamentDto {
        public Guid Id { get; set; } 
        public string Name { get; set; } = ""; 
        public DateTime BeginDate { get; set; } 
        public DateTime EndDate { get; set; } 
        public int Prizepool { get; set; } 
    }
    public class ResultDto
    {   
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid MatchId { get; set; }
        public int Place { get; set; }
        public int Points { get; set; }
        public int PrizeMoney { get; set; }
    }

    public class ApiClient
    {
        private static ApiClient? _instance;

        public static ApiClient Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ApiClient();
                return _instance;
            }
        }

        private readonly HttpClient _httpClient;
        private string? _authToken;

        public async Task<List<TournamentDto>> GetTournamentsAsync(bool useSql = false)
        {
            try { return await _httpClient.GetFromJsonAsync<List<TournamentDto>>($"api/tournaments?useSql={useSql}") ?? new List<TournamentDto>(); }
            catch { return new List<TournamentDto>(); }
        }

        public async Task<bool> CreateTournamentAsync(string name, int prizepool, bool useSql = false)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/tournaments?useSql={useSql}",
                new { Name = name, BeginDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(7), Prizepool = prizepool });
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ResultDto>> GetResultsAsync(bool useSql = false)
        {
            try { return await _httpClient.GetFromJsonAsync<List<ResultDto>>($"api/tournaments/results?useSql={useSql}") ?? new List<ResultDto>(); }
            catch { return new List<ResultDto>(); }
        }

        public async Task<bool> RecordResultAsync(int place, int points, int prizeMoney, Guid matchId, Guid teamId, bool useSql = false)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/tournaments/results?useSql={useSql}",
                new { Place = place, Points = points, PrizeMoney = prizeMoney, MatchId = matchId, TeamId = teamId });
            return response.IsSuccessStatusCode;
        }

        private ApiClient()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5007/") };
        }

        public async Task<bool> LoginAsync(string user, string pass)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Username = user, Password = pass });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                _authToken = result?.Token;
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
                return true;
            }
            return false;
        }
        public async Task<List<PlayerDto>> GetPlayersAsync(string search = "", bool useSql = false)
        {
            try
            {
                var url = $"api/players?useSql={useSql}";
                if (!string.IsNullOrEmpty(search)) url += $"&search={search}";
                var players = await _httpClient.GetFromJsonAsync<List<PlayerDto>>(url);
                return players ?? new List<PlayerDto>();
            }
            catch { return new List<PlayerDto>(); }
        }

        public async Task<bool> CreatePlayerAsync(string nick, string first, string last, bool useSql = false)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/players?useSql={useSql}",
                new { Nickname = nick, FirstName = first, LastName = last, Phone = 123456, DateOfBirth = DateTime.UtcNow.AddYears(-20) });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeletePlayerAsync(Guid id, bool useSql = false)
        {
            var response = await _httpClient.DeleteAsync($"api/players/{id}?useSql={useSql}");
            return response.IsSuccessStatusCode;
        }
        public async Task<List<TeamDto>> GetTeamsAsync(string search = "", bool useSql = false)
        {
            try
            {
                var url = $"api/teams?useSql={useSql}";

                if (!string.IsNullOrEmpty(search))
                {
                    url += $"&search={search}";
                }

                var teams = await _httpClient.GetFromJsonAsync<List<TeamDto>>(url);
                return teams ?? new List<TeamDto>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к серверу: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<TeamDto>();
            }
        }

        public async Task<bool> CreateTeamAsync(string name, Guid gameId, bool useSql = false)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/teams?useSql={useSql}", new { Name = name, GameId = gameId });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTeamAsync(Guid id, string name, Guid gameId, bool useSql = false)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/teams/{id}?useSql={useSql}", new { Name = name, GameId = gameId });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTeamAsync(Guid id, bool useSql = false)
        {
            var response = await _httpClient.DeleteAsync($"api/teams/{id}?useSql={useSql}");
            return response.IsSuccessStatusCode;
        }
    }

    public class LoginResponse { public string Token { get; set; } = ""; }
}