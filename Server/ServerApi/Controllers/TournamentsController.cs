using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApi.Data;
using ServerApi.Models;

namespace ServerApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentsController : ControllerBase
    {
        private readonly EsportsContext _context;
        public TournamentsController(EsportsContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetTournaments([FromQuery] string? search, [FromQuery] bool useSql = false)
        {
            List<Tournament> tournaments;
            if (useSql)
            {
                tournaments = string.IsNullOrEmpty(search)
                    ? await _context.Tournaments.FromSqlRaw("SELECT * FROM tournament").ToListAsync()
                    : await _context.Tournaments.FromSqlRaw("SELECT * FROM tournament WHERE name ILIKE {0}", $"%{search}%").ToListAsync();
            }
            else
            {
                var query = _context.Tournaments.AsQueryable();
                if (!string.IsNullOrEmpty(search)) query = query.Where(t => t.Name.ToLower().Contains(search.ToLower()));
                tournaments = await query.ToListAsync();
            }
            return Ok(tournaments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTournament([FromBody] Tournament dto, [FromQuery] bool useSql = false)
        {
            var newId = Guid.NewGuid();
            if (useSql)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO tournament (id, start_date, end_date, name, prizepool) VALUES ({0}, {1}, {2}, {3}, {4})",
                    newId, dto.BeginDate, dto.EndDate, dto.Name, dto.Prizepool);
            }
            else
            {
                dto.Id = newId;
                _context.Tournaments.Add(dto);
                await _context.SaveChangesAsync();
            }
            return StatusCode(201, dto);
        }

        [HttpGet("results")]
        public async Task<IActionResult> GetResults([FromQuery] bool useSql = false)
        {
            var results = useSql
                ? await _context.MatchResults.FromSqlRaw("SELECT * FROM match_result").ToListAsync()
                : await _context.MatchResults.ToListAsync();
            return Ok(results);
        }

        [HttpPost("results")]
        public async Task<IActionResult> RecordResult([FromBody] MatchResult dto, [FromQuery] bool useSql = false)
        {
            var newId = Guid.NewGuid();
            if (useSql)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO match_result (id, team_id, match_id, place, points, prize_money) VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                    newId, dto.TeamId, dto.MatchId, dto.Place, dto.Points, dto.PrizeMoney);
            }
            else
            {
                dto.Id = newId;
                _context.MatchResults.Add(dto);
                await _context.SaveChangesAsync();
            }
            return StatusCode(201, dto);
        }
    }
}