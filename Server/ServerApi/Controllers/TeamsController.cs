using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApi.Data;
using ServerApi.DTOs; // Обязательно подключаем DTO
using ServerApi.Models;

namespace ServerApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly EsportsContext _context;
        public TeamsController(EsportsContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetTeams([FromQuery] string? search, [FromQuery] bool useSql = false)
        {
            List<Team> teams;
            if (useSql)
            {
                teams = string.IsNullOrEmpty(search)
                    ? await _context.Teams.FromSqlRaw("SELECT * FROM team").Include(t => t.TeamPlayers).ToListAsync()
                    : await _context.Teams.FromSqlRaw("SELECT * FROM team WHERE name ILIKE {0}", $"%{search}%").Include(t => t.TeamPlayers).ToListAsync();
            }
            else
            {
                var query = _context.Teams.Include(t => t.TeamPlayers).AsQueryable();
                if (!string.IsNullOrEmpty(search)) query = query.Where(t => t.Name.ToLower().Contains(search.ToLower()));
                teams = await query.ToListAsync();
            }

            // МАГИЯ ЗДЕСЬ: Превращаем сложные модели в простые DTO, отрезая циклы
            var teamDtos = teams.Select(t => new TeamDTO
            {
                Id = t.Id,
                Name = t.Name,
                GameId = t.GameId,
                PlayersCount = t.TeamPlayers?.Count ?? 0
            }).ToList();

            return Ok(teamDtos); // Отправляем безопасные DTO
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] TeamCreateDto dto, [FromQuery] bool useSql = false)
        {
            var newId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;

            if (useSql)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO team (id, name, game_id, created_at) VALUES ({0}, {1}, {2}, {3})",
                    newId, dto.Name, dto.GameId, createdAt);
            }
            else
            {
                var team = new Team { Id = newId, Name = dto.Name, GameId = dto.GameId, CreatedAt = createdAt };
                _context.Teams.Add(team);
                await _context.SaveChangesAsync();
            }

            return StatusCode(201, dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(Guid id, [FromQuery] bool useSql = false)
        {
            if (useSql) await _context.Database.ExecuteSqlRawAsync("DELETE FROM team WHERE id = {0}", id);
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