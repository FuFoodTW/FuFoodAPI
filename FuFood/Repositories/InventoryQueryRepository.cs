using FuFood.Models.Enums;
using FuFood.Models.Requests;
using FuFood.Data;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class InventoryQueryRepository(AppDbContext dbContext)
{
    public async Task<Dictionary<Guid, decimal>> GetRemainingInventoryForItems(IEnumerable<Guid> itemIds)
    {
        return await dbContext.InventoryTransactionsItems
            .Where(iti => iti.ParentId == null && iti.FullyConsumedAt == null && itemIds.Contains(iti.Id))
            .GroupJoin(
                dbContext.InventoryTransactionsItems.Where(child => child.ParentId != null),
                parent => parent.Id,
                child => child.ParentId,
                (parent, children) => new
                {
                    Id = parent.Id,
                    Remaining = parent.Quantity + (children.Sum(c => (decimal?)c.Quantity) ?? 0)
                }
            )
            .ToDictionaryAsync(x => x.Id, x => x.Remaining);
    }

    public async Task<decimal?> GetRemainingInventoryForItems(Guid itemId)
    {
        var map = await GetRemainingInventoryForItems([itemId]);
        return map[itemId];
    }

    public async Task<List<InventoryItemDto>> GetInventoryByCategory(Guid refrigeratorId, ProductCategory category)
    {
        var items = await dbContext.InventoryTransactionsItems.Where(i =>
                i.InventoryTransaction!.RefrigeratorId == refrigeratorId && i.ParentId == null &&
                (i.Product!.Categories & category) == category && i.FullyConsumedAt == null
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
                RemainingQuantity = i.Quantity
            })
            .OrderByDescending(x => x.ItemId)
            .ToListAsync();

        // Get all item IDs to perform the second database query
        var itemIds = items.Select(i => i.ItemId);
        // Get actual remaining quantities for all items' IDs
        var remainingQuantities = await GetRemainingInventoryForItems(itemIds);

        // Iterate over the list and set the actual values from the second query's result
        foreach (var item in items)
            if (remainingQuantities.TryGetValue(item.ItemId, out var stock))
                item.RemainingQuantity = stock;

        return items;
    }

    public async Task<ProductInventoryDetailDto?> GetProductInventoryDetail(Guid refrigeratorId, Guid productId)
    {
        var product = await dbContext.Products.FindAsync(productId);
        if (product == null) return null;

        var batches = await dbContext.InventoryTransactionsItems.Where(i =>
                i.ProductId == productId && i.ParentId == null &&
                i.InventoryTransaction!.RefrigeratorId == refrigeratorId)
            .Select(i => new InventoryBatchDto
            {
                ItemId = i.Id,
                ExpirationDate = i.ExpirationDate,
                CreatedAt = i.CreatedAt,
                RemainingQuantity = i.Quantity
            })
            .Where(x => x.RemainingQuantity > 0)
            .OrderByDescending(x => x.ExpirationDate)
            .ToListAsync();

        var itemIds = batches.Select(i => i.ItemId);
        var remainingQuantities = await GetRemainingInventoryForItems(itemIds);

        // Iterate over the list and set the actual values from the second query's result
        foreach (var batch in batches)
            if (remainingQuantities.TryGetValue(batch.ItemId, out var stock))
                batch.RemainingQuantity = stock;

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