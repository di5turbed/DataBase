namespace ServerApi.DTOs
{
    public class TeamCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid GameId { get; set; }
    }
}
