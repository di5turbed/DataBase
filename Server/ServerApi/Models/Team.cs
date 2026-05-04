using System.ComponentModel.DataAnnotations;

namespace ServerApi.Models
{
    public class Team
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        public Guid GameId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Навигационное свойство
        public ICollection<TeamPlayer> TeamPlayers { get; set; } = new List<TeamPlayer>();
    }
}