using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServerApi.Data;
using ServerApi.Models;
using Xunit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace ServerApi.Tests
{
    public class TournamentsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TournamentsIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var testFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        {"ASPNETCORE_ENVIRONMENT", "Testing"}
                    });
                });
            });

            _client = testFactory.CreateClient();
        }

        private async Task AuthenticateAsync()
        {
            var credentials = new { Username = "e2e_admin", Password = "testpassword" };
            
            await _client.PostAsJsonAsync("/api/auth/register", credentials);
            
            var response = await _client.PostAsJsonAsync("/api/auth/login", credentials);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var token = json.GetProperty("token").GetString();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task FullTournamentLifecycle_CreateAndRetrieve_ReturnsSuccess()
        {
            await AuthenticateAsync();

            var newTournament = new Tournament
            {
                Name = "The International E2E",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(10),
                MaxParticipants = 32
            };

            var postResponse = await _client.PostAsJsonAsync("/api/tournaments?useSql=false", newTournament);
            
            postResponse.EnsureSuccessStatusCode(); 

            var getResponse = await _client.GetAsync("/api/tournaments?useSql=false");
            getResponse.EnsureSuccessStatusCode();

            var tournaments = await getResponse.Content.ReadFromJsonAsync<List<Tournament>>();

            Assert.NotNull(tournaments);
            Assert.Contains(tournaments, t => t.Name == "The International E2E");
            Assert.Contains(tournaments, t => t.MaxParticipants == 32);
        }
    }
}