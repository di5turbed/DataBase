using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApi.Models
{
    [Table("match_result")]
    public class MatchResult
    {
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("winner_team")]
        public Guid WinnerTeam { get; set; }

        [Column("tournament_id")]
        public Guid TournamentId { get; set; }

        [Column("total_prize_money")]
        public int TotalPrizeMoney { get; set; }
    }
}