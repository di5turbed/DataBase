using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApi.Data;
using ServerApi.DTOs;
using ServerApi.Models;

namespace ServerApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly EsportsContext _context;

        public TeamsController(EsportsContext context)
        {
            _context = context;
        }

        // 1. ЧТЕНИЕ И ПОИСК С ФИЛЬТРОМ
        [HttpGet]
        public async Task<IActionResult> GetTeams([FromQuery] string? search, [FromQuery] bool useSql = false)
        {
            List<Team> teams;

            if (useSql)
            {
                // Вариант 1: ЧИСТЫЙ SQL (ILIKE - независим от регистра в Postgres)
                if (string.IsNullOrEmpty(search))
                {
                    teams = await _context.Teams.FromSqlRaw("SELECT * FROM team").Include(t => t.TeamPlayers).ToListAsync();
                }
                else
                {
                    teams = await _context.Teams.FromSqlRaw("SELECT * FROM team WHERE name ILIKE {0}", $"%{search}%").Include(t => t.TeamPlayers).ToListAsync();
                }
            }
            else
            {
                // Вариант 2: ORM
                var query = _context.Teams.Include(t => t.TeamPlayers).AsQueryable();
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(t => t.Name.ToLower().Contains(search.ToLower()));
                }
                teams = await query.ToListAsync();
            }

            return Ok(teams.Select(t => new TeamDTO { Id = t.Id, Name = t.Name, PlayersCount = t.TeamPlayers.Count }));
        }

        // 2. ДОБАВЛЕНИЕ (Create)
        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] TeamCreateDto dto, [FromQuery] bool useSql = false)
        {
            if (useSql)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO team (id, name, game_id, created_at) VALUES ({0}, {1}, {2}, {3})",
                    Guid.NewGuid(), dto.Name, dto.GameId, DateTime.UtcNow);
            }
            else
            {
                _context.Teams.Add(new Team { Name = dto.Name, GameId = dto.GameId, CreatedAt = DateTime.UtcNow });
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        // 3. ИЗМЕНЕНИЕ (Update)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(Guid id, [FromBody] TeamCreateDto dto, [FromQuery] bool useSql = false)
        {
            if (useSql)
            {
                await _context.Database.ExecuteSqlRawAsync("UPDATE team SET name = {0}, game_id = {1} WHERE id = {2}", dto.Name, dto.GameId, id);
            }
            else
            {
                var team = await _context.Teams.FindAsync(id);
                if (team == null) return NotFound();
                team.Name = dto.Name;
                team.GameId = dto.GameId;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        // 4. УДАЛЕНИЕ (Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(Guid id, [FromQuery] bool useSql = false)
        {
            if (useSql)
            {
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM team WHERE id = {0}", id);
            }
            else
            {
                var team = await _context.Teams.FindAsync(id);
                if (team == null) return NotFound();
                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
    }
}