using Microsoft.EntityFrameworkCore;
using ServerApi.Models;

namespace ServerApi.Data
{
    public class EsportsContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamPlayer> TeamPlayers { get; set; }
        
        // Сюда потом добавишь DbSet для Tournament, Match, Hall и т.д.

        public EsportsContext(DbContextOptions<EsportsContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Player>().ToTable("players");
            modelBuilder.Entity<Team>().ToTable("team");
            modelBuilder.Entity<TeamPlayer>().ToTable("team_player");

            modelBuilder.Entity<TeamPlayer>()
                .HasOne(tp => tp.Team)
                .WithMany(t => t.TeamPlayers)
                .HasForeignKey(tp => tp.TeamId);

            modelBuilder.Entity<TeamPlayer>()
                .HasOne(tp => tp.Player)
                .WithMany(p => p.TeamPlayers)
                .HasForeignKey(tp => tp.PlayerId);
        }
    }
}