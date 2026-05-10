using System.Net.Http.Json;

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
        private readonly HttpClient _httpClient;
        private string? _authToken;

        public ApiClient()
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
        public async Task<List<TeamDto>> GetTeamsAsync()
        {
            try
            {
                var teams = await _httpClient.GetFromJsonAsync<List<TeamDto>>("api/teams");
                return teams ?? new List<TeamDto>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к серверу: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<TeamDto>();
            }
        }
    }
    public class LoginResponse { public string Token { get; set; } = ""; }
}