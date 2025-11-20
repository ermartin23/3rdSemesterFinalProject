using Microsoft.EntityFrameworkCore;
using DataAccess.Models;

namespace DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Board> Boards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        

        modelBuilder.Entity<Game>()
            .Property(g => g.DrawnNumbers)
            .HasColumnType("jsonb");

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(10, 2);
    }
}