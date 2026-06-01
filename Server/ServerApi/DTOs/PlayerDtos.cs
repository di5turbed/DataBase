namespace ServerApi.DTOs
{
    public class PlayerDto
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class PlayerCreateDto
    {
        public string Nickname { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}