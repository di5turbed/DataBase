using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ServerApi.Controllers;
using Xunit;

namespace ServerApi.Tests
{
    public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsSuccess()
        {
            var randomUsername = "test_user_" + Guid.NewGuid().ToString().Substring(0, 8);
            var request = new RegisterRequest
            {
                Username = randomUsername,
                Password = "TestPassword123!"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", request);

            response.EnsureSuccessStatusCode(); 
            
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("успешно", responseString);
        }
    }
}