using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApi.Controllers;
using ServerApi.Data;
using ServerApi.DTOs;
using ServerApi.Models;
using Xunit;

namespace ServerApi.Tests
{
    public class TeamsControllerTests
    {
        private DbContextOptions<EsportsContext> GetInMemoryOptions()
        {
            return new DbContextOptionsBuilder<EsportsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя для каждого теста
                .Options;
        }

        [Fact]
        public async Task GetTeams_ReturnsOkResult_WithData()
        {
            var options = GetInMemoryOptions();
            
            using (var context = new EsportsContext(options))
            {
                context.Teams.Add(new Team { Id = Guid.NewGuid(), Name = "Navi Test", CreatedAt = DateTime.UtcNow });
                context.SaveChanges();
            }

            using (var context = new EsportsContext(options))
            {
                var controller = new TeamsController(context);

                var result = await controller.GetTeams(search: null, useSql: false);

                var okResult = Assert.IsType<OkObjectResult>(result);
                
                var teams = Assert.IsAssignableFrom<IEnumerable<TeamDTO>>(okResult.Value);
                
                Assert.Single(teams);
                Assert.Equal("Navi Test", teams.First().Name);
            }
        }
    }
}