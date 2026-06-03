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
            // Указываем точные названия таблиц в PostgreSQL (в нижнем регистре)
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Team>().ToTable("team");
            modelBuilder.Entity<Player>().ToTable("players");
            modelBuilder.Entity<TeamPlayer>().ToTable("team_player");

            // ПРИВЯЗКА НОВЫХ ТАБЛИЦ:
            modelBuilder.Entity<Tournament>().ToTable("tournament");
            modelBuilder.Entity<MatchResult>().ToTable("match_result");
        }
    }
}