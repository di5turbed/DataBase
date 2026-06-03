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
                players = string.IsNullOrEmpty(search)
                    ? await _context.Players.FromSqlRaw("SELECT * FROM player").ToListAsync()
                    : await _context.Players.FromSqlRaw("SELECT * FROM player WHERE nickname ILIKE {0}", $"%{search}%").ToListAsync();
            }
            else
            {
                var query = _context.Players.AsQueryable();
                if (!string.IsNullOrEmpty(search)) query = query.Where(p => p.Nickname.ToLower().Contains(search.ToLower()));
                players = await query.ToListAsync();
            }
            return Ok(players);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromBody] Player dto, [FromQuery] bool useSql = false)
        {
            var newId = Guid.NewGuid();
            var regDate = DateTime.UtcNow;

            if (useSql)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO player (id, nickname, first_name, last_name, phone, reg_date, date_of_birth) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                    newId, dto.Nickname, dto.FirstName, dto.LastName, dto.Phone, regDate, dto.DateOfBirth);
            }
            else
            {
                dto.Id = newId;
                dto.RegDate = regDate;
                _context.Players.Add(dto);
                await _context.SaveChangesAsync();
            }
            // Возвращаем правильный статус 201 Created
            return StatusCode(201, dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(Guid id, [FromQuery] bool useSql = false)
        {
            if (useSql) await _context.Database.ExecuteSqlRawAsync("DELETE FROM player WHERE id = {0}", id);
            else
            {
                var player = await _context.Players.FindAsync(id);
                if (player == null) return NotFound("Игрок не найден");
                _context.Players.Remove(player);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
    }
}