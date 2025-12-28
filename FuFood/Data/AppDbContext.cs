using FuFood.Models;
using FuFood.Models.Entities;
using FuFood.Models.Interfaces;
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
    public DbSet<RefrigeratorMembership> RefrigeratorMemberships { get; set; }
    public DbSet<RefrigeratorInvitation> RefrigeratorInvitations { get; set; }

    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries<IHasTimestamp>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RefrigeratorMembership>()
            .HasOne(rm => rm.Refrigerator)
            .WithMany(r => r.Memberships)
            .HasForeignKey(rm => rm.RefrigeratorId);

        modelBuilder.Entity<RefrigeratorMembership>()
            .HasOne(rm => rm.Member)
            .WithMany()
            .HasForeignKey(rm => rm.MemberId);

        modelBuilder.Entity<Refrigerator>()
            .HasIndex(r => r.OwnerId)
            .IsUnique()
            .HasFilter("\"IsDefault\" = true");

        modelBuilder.Entity<InventoryTransactionItem>()
            .HasIndex(r => new { r.ParentId, r.InventoryTransactionId })
            .IsUnique()
            .HasFilter("\"ParentId\" is not null");

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