using FuFood.Data;
using FuFood.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class ProductRepository(AppDbContext dbContext)
{
    public async Task<Product?> GetById(Guid productId)
    {
        return await dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
    }
}