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
    public class PlayersController : ControllerBase
    {
        private readonly EsportsContext _context;
        public PlayersController(EsportsContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetPlayers([FromQuery] string? search, [FromQuery] bool useSql = false)
        {
            List<Player> players;
            if (useSql)
            {
                if (string.IsNullOrEmpty(search))
                    players = await _context.Players.FromSqlRaw("SELECT * FROM players").ToListAsync();
                else
                    players = await _context.Players.FromSqlRaw("SELECT * FROM players WHERE nickname ILIKE {0}", $"%{search}%").ToListAsync();
            }
            else
            {
                var query = _context.Players.AsQueryable();
                if (!string.IsNullOrEmpty(search)) query = query.Where(p => p.Nickname.ToLower().Contains(search.ToLower()));
                players = await query.ToListAsync();
            }
            return Ok(players.Select(p => new PlayerDto { Id = p.Id, Nickname = p.Nickname, FirstName = p.FirstName, LastName = p.LastName }));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromBody] PlayerCreateDto dto, [FromQuery] bool useSql = false)
        {
            if (useSql)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO players (id, nickname, first_name, last_name, phone, reg_date, date_of_birth) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                    Guid.NewGuid(), dto.Nickname, dto.FirstName, dto.LastName, dto.Phone, DateTime.UtcNow, dto.DateOfBirth);
            }
            else
            {
                _context.Players.Add(new Player { Nickname = dto.Nickname, FirstName = dto.FirstName, LastName = dto.LastName, Phone = dto.Phone, RegDate = DateTime.UtcNow, DateOfBirth = dto.DateOfBirth });
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(Guid id, [FromQuery] bool useSql = false)
        {
            if (useSql) await _context.Database.ExecuteSqlRawAsync("DELETE FROM players WHERE id = {0}", id);
            else
            {
                var player = await _context.Players.FindAsync(id);
                if (player != null) { _context.Players.Remove(player); await _context.SaveChangesAsync(); }
            }
            return Ok();
        }
    }
}