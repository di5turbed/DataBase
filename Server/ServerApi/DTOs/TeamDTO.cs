namespace ServerApi.DTOs
{
    public class TeamDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid GameId { get; set; }
        public int PlayersCount { get; set; }
    }
}
