using FuFood.Models;
using FuFood.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Refrigerator> Refrigerators { get; set; }
    public DbSet<RevokedAccessToken> RevokedAccessTokens { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
    public DbSet<InventoryTransactionItem> InventoryTransactionsItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Refrigerator>()
            .HasIndex(r => r.CreatedById)
            .IsUnique()
            .HasFilter("\"IsDefault\" = true");
    }
}