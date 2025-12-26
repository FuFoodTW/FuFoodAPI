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

        // Add check constraints to enforce data validity on the database level
        modelBuilder.Entity<InventoryTransactionItem>()
            .ToTable(t =>
            {
                // storing transaction items must have quantity > 0
                // consuming transaction items must have quantity < 0
                t.HasCheckConstraint("item_quantity_sign_check",
                    "(\"ParentId\" is null and \"Quantity\" > 0) or (\"ParentId\" is not null and \"Quantity\" < 0)");

                // expiration date is only required for parent items,
                // it is redundant for consuming transactions
                t.HasCheckConstraint("item_expiration_date_check",
                    "\"ParentId\" is not null or \"ExpirationDate\" is not null");
            });
    }
}