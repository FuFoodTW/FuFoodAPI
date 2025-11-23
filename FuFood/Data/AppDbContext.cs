using FuFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}