using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApi.Models
{
    [Table("match_result")]
    public class MatchResult
    {
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("result")]
        public string Result { get; set; } = string.Empty;

        [Column("kills")]
        public int Kills { get; set; }

        [Column("deaths")]
        public int Deaths { get; set; }

        [Column("match_id")]
        public Guid MatchId { get; set; }

        [Column("team_id")]
        public Guid TeamId { get; set; }
    }
}