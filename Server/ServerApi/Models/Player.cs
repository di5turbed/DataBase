using System.ComponentModel.DataAnnotations;

namespace ServerApi.Models
{
    public class Player
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [MaxLength(100)]
        public string Nickname { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Phone { get; set; } // Согласно ER-диаграмме тип int
        public DateTime RegDate { get; set; }
        public DateTime DateOfBirth { get; set; }

        // Навигационное свойство
        public ICollection<TeamPlayer> TeamPlayers { get; set; } = new List<TeamPlayer>();
    }
}