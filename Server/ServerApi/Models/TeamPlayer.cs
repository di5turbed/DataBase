namespace ServerApi.Models
{
    public class TeamPlayer
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // У тебя на схеме у Team_Player есть свой собственный id
        public Guid TeamId { get; set; }
        public Guid PlayerId { get; set; }
        public DateTime JoinDate { get; set; }

        public Team Team { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}