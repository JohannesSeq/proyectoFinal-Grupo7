using Microsoft.EntityFrameworkCore;
using QuinxoWebApp.Models;

namespace QuinxoWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GamePlayer> GamePlayers { get; set; }
        public DbSet<Move> Moves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.WinnerPlayer)
                .WithMany()
                .HasForeignKey(g => g.WinnerPlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GamePlayer>()
                .HasOne(gp => gp.Game)
                .WithMany(g => g.GamePlayers)
                .HasForeignKey(gp => gp.GameId);

            modelBuilder.Entity<GamePlayer>()
                .HasOne(gp => gp.Player)
                .WithMany(p => p.GamePlayers)
                .HasForeignKey(gp => gp.PlayerId);

            modelBuilder.Entity<Move>()
                .HasOne(m => m.Game)
                .WithMany(g => g.Moves)
                .HasForeignKey(m => m.GameId);

            modelBuilder.Entity<Move>()
                .HasOne(m => m.Player)
                .WithMany()
                .HasForeignKey(m => m.PlayerId);
        }
    }
}
