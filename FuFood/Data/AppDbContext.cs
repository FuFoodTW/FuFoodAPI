using FuFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RevokedAccessToken> RevokedAccessTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RevokedAccessToken>()
            .Property(x => x.Id)
            .ValueGeneratedNever();
    }
}