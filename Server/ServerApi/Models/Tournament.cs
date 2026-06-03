using System.ComponentModel.DataAnnotations.Schema;

namespace ServerApi.Models
{
    [Table("tournament")]
    public class Tournament
    {
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Column("start_date")]
        public DateTime BeginDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("prizepool")]
        public int Prizepool { get; set; }
    }
}