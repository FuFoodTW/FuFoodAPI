using FuFood.Models.Enums;
using FuFood.Models.Requests;
using FuFood.Data;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class InventoryQueryRepository(AppDbContext dbContext)
{
    public async Task<List<InventoryItemDto>> GetInventoryByCategory(Guid refrigeratorId, ProductCategory category)
    {
        var items = await dbContext.InventoryTransactionsItems.Where(i =>
                i.InventoryTransaction!.RefrigeratorId == refrigeratorId && i.ParentId == null &&
                (i.Product!.Categories & category) == category
            )
            .Include(i => i.Product)
            .Include(i => i.InventoryTransaction).Select(i => new InventoryItemDto
            {
                ItemId = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product!.Name,
                Category = i.Product.Categories,
                Unit = i.Product.Unit,
                ExpirationDate = i.ExpirationDate,
                CreatedAt = i.CreatedAt,

                RemainingQuantity =
                    i.Quantity - dbContext.InventoryTransactionsItems
                        .Where(c => c.ParentId == i.Id).Sum(c => c.Quantity)
            }).Where(x => x.RemainingQuantity > 0)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return items;
    }

    public async Task<ProductInventoryDetailDto?> GetProductInventoryDetail(Guid refrigeratorId, Guid productId)
    {
        var product = await dbContext.Products.FindAsync(productId);
        if (product == null) return null;

        var batches = await dbContext.InventoryTransactionsItems.Where(i =>
                i.ParentId == productId && i.ParentId == null &&
                i.InventoryTransaction!.RefrigeratorId == refrigeratorId)
            .Select(i => new InventoryBatchDto
            {
                ItemId = i.Id,
                ExpirationDate = i.ExpirationDate,
                CreatedAt = i.CreatedAt,
                RemainingQuantity = i.Quantity - dbContext.InventoryTransactionsItems.Where(c => c.ParentId == i.Id)
                    .Sum(c => c.Quantity)
            })
            .Where(x => x.RemainingQuantity > 0)
            .OrderByDescending(x => x.ExpirationDate
            ).ToListAsync();

        return new ProductInventoryDetailDto
        {
            ProductId = product.Id,
            Name = product.Name,
            Unit = product.Unit,
            Categories = product.Categories,
            Batches = batches
        };
    }
}