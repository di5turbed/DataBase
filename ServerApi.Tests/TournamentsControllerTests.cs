using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApi.Controllers;
using ServerApi.Data;
using ServerApi.Models;
using Xunit;

namespace ServerApi.Tests
{
    public class TournamentsControllerTests
    {
        private EsportsContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<EsportsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new EsportsContext(options);
        }

        [Fact]
        public async Task CreateTournament_ORMMode_SavesTournamentCorrectly()
        {
            using var context = GetInMemoryContext();
            var controller = new TournamentsController(context);
            var newTournament = new Tournament
            {
                Name = "Epicenter 2026",
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(5),
                MaxParticipants = 16
            };

            var result = await controller.CreateTournament(newTournament, useSql: false);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, statusCodeResult.StatusCode);

            var dbTournament = await context.Tournaments.FirstOrDefaultAsync(t => t.Name == "Epicenter 2026");
            Assert.NotNull(dbTournament);
            Assert.Equal(16, dbTournament.MaxParticipants);
        }

        [Fact]
        public async Task RecordResult_ORMMode_SavesMatchResultCorrectly()
        {
            using var context = GetInMemoryContext();
            var controller = new TournamentsController(context);
            var matchResultDto = new MatchResult
            {
                WinnerTeam = Guid.NewGuid(),
                TournamentId = Guid.NewGuid(),
                TotalPrizeMoney = 500000
            };

            var result = await controller.RecordResult(matchResultDto, useSql: false);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, statusCodeResult.StatusCode);

            var dbResult = await context.MatchResults.FirstOrDefaultAsync(r => r.TotalPrizeMoney == 500000);
            Assert.NotNull(dbResult);
            Assert.Equal(matchResultDto.WinnerTeam, dbResult.WinnerTeam);
        }
    }
}