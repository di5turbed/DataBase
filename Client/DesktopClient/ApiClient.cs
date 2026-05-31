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