using Microsoft.EntityFrameworkCore;
using ServerApi.Models;

namespace ServerApi.Data
{
    public class EsportsContext : DbContext
    {
        public EsportsContext(DbContextOptions<EsportsContext> options) : base(options) { }

        // Зарегистрированные таблицы базы данных
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<TeamPlayer> TeamPlayers { get; set; }

        // НОВЫЕ ТАБЛИЦЫ: Турниры и Результаты
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<MatchResult> MatchResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName()?.ToLower());

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToLower());
                }
            }
        }
    }
}