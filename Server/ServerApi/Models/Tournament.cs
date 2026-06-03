using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApi.Models
{
    [Table("tournament")]
    public class Tournament
    {
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }

        [Column("max_participants")]
        public int MaxParticipants { get; set; }
    }
}