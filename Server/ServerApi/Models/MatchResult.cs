using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApi.Models
{
    [Table("match_result")]
    public class MatchResult
    {
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("team_id")]
        public Guid TeamId { get; set; }

        [Column("match_id")]
        public Guid MatchId { get; set; }

        [Column("place")]
        public int Place { get; set; }

        [Column("points")]
        public int Points { get; set; }

        [Column("prize_money")]
        public int PrizeMoney { get; set; }
    }
}