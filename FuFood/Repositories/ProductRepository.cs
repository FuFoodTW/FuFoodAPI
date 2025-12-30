using FuFood.Data;
using FuFood.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class ProductRepository(AppDbContext dbContext)
{
    public IQueryable<Product> GetProductsQueryByRefrigerator(Guid refrigeratorId)
    {
        return dbContext.InventoryTransactionsItems
            .Where(i => i.InventoryTransaction!.RefrigeratorId == refrigeratorId)
            .Select(i => i.Product!)
            .Distinct();
    }

    public async Task<List<Product>> GetProductsByRefrigeratorAsync(Guid refrigeratorId)
    {
        return await GetProductsQueryByRefrigerator(refrigeratorId).ToListAsync();
    }

    public async Task<Product?> GetById(Guid productId)
    {
        return await dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
    }
}