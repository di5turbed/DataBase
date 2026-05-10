using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApi.Data;
using ServerApi.DTOs;

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

        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            var teams = await _context.Teams
                .Include(t => t.TeamPlayers)
                .Select(t => new TeamDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    PlayersCount = t.TeamPlayers.Count
                })
                .ToListAsync();

            return Ok(teams);
        }
    }
}