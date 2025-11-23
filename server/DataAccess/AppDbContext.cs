using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("DeadPigeonsDB");

            modelBuilder.Entity<Player>()
                .HasKey(p => p.PlayerId);

            modelBuilder.Entity<Game>()
                .HasKey(g => g.GameId);

            modelBuilder.Entity<Game>()
                .Property(g => g.WinningNumbers)
                .HasColumnType("integer[]");

            modelBuilder.Entity<Game>()
                .Property(g => g.CutoffTime)
                .HasConversion(
                    v => v.ToString(),
                    v => TimeOnly.Parse(v)
                );

            modelBuilder.Entity<Board>()
                .HasKey(b => b.BoardId);

            modelBuilder.Entity<Board>()
                .HasOne(b => b.Player)
                .WithMany()
                .HasForeignKey(b => b.PlayerId);

            modelBuilder.Entity<Board>()
                .HasOne(b => b.Game)
                .WithMany()
                .HasForeignKey(b => b.GameId);

            modelBuilder.Entity<Transaction>()
                .HasKey(t => t.TransactionId);

            modelBuilder
                .Entity<Transaction>()
                .Property(t => t.Status)
                .HasConversion(
                    new EnumToStringConverter<TransactionStatus>()
                )
                .HasColumnType("transaction_status");

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Player)
                .WithMany()
                .HasForeignKey(t => t.PlayerId);
        }
    }
}
