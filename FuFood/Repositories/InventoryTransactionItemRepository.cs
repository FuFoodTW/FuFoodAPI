using FuFood.Data;
using FuFood.Models.Entities;
using FuFood.Queries;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class InventoryTransactionItemRepository(AppDbContext dbContext, InventoryQueryRepository queryRepository)
{
    public async Task<InventoryTransactionItem> Create(InventoryTransactionItem item)
    {
        dbContext.InventoryTransactionsItems.Add(item);
        await dbContext.SaveChangesAsync();
        return item;
    }

    // 新增入庫項目
    public async Task<InventoryTransactionItem> Create(
        InventoryTransaction transaction, Product product, decimal quantity, DateOnly expirationDate,
        string? image = null, Guid? parentId = null)
    {
        var item = new InventoryTransactionItem
        {
            InventoryTransactionId = transaction.Id,
            ProductId = product.Id,
            Quantity = quantity,
            ExpirationDate = expirationDate,
            InventoryTransactionItemImage = image,
            ParentId = parentId
        };

        dbContext.InventoryTransactionsItems.Add(item);
        await dbContext.SaveChangesAsync();
        return item;
    }

    // 找到該筆交易,返回其 items
    public async Task<List<InventoryTransactionItem>> GetByTransactionId(Guid transactionId)
    {
        return await dbContext.InventoryTransactionsItems.Where(i => i.InventoryTransactionId == transactionId)
            .Include(i => i.ProductId).ToListAsync();
    }

    // 取得該 item
    public async Task<InventoryTransactionItem?> GetById(Guid itemId)
    {
        return await dbContext.InventoryTransactionsItems.Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == itemId);
    }

    // 建立消耗 item
    public async Task<InventoryTransactionItem> Consume(InventoryTransaction transaction, Guid inventoryItemId,
        decimal quantity)
    {
        if (quantity <= 0) throw new InvalidOperationException("Quantity must be greater than 0");

        var parentItem = await dbContext.InventoryTransactionsItems
            .Consumable()
            .FirstOrDefaultAsync(i => i.Id == inventoryItemId);

        if (parentItem == null) throw new Exception("Inventory item not found");

        var remaining = await queryRepository.GetRemainingInventoryForItems(inventoryItemId);

        if (quantity > remaining)
            throw new InvalidOperationException(
                $"The requested consumption quantity {quantity} is greater than the remaining inventory {remaining}.");

        await dbContext.Database.BeginTransactionAsync();

        // 
        var items = await dbContext.InventoryTransactionsItems
            .FromSqlRaw(
                """
                insert into "InventoryTransactionItems" iti
                ("Id", "ParentId", "ProductId", "Quantity", "InventoryTransactionId", "CreatedAt", "UpdatedAt")
                values (uuidv7(), {0}, {1}, {2}, {3}, now() at time zone 'utc', now() at time zone 'utc')
                on conflict ("ParentId", "InventoryTransactionId")
                do update set "Quantity" = "Quantity" + EXCLUDED."Quantity", "UpdatedAt" = EXCLUDED."UpdatedAt"
                where ABS(iti."Quantity" + EXCLUDED."Quantity") <= {4}
                returning iti.*;
                """,
                parentItem.Id, parentItem.ProductId, -quantity, transaction.Id, remaining)
            .ToListAsync();

        var item = items.FirstOrDefault();

        if (item != null) return items.First();

        await dbContext.Database.RollbackTransactionAsync();
        throw new InvalidOperationException(
            $"The requested consumption quantity combined with existing items exceeds the remaining inventory {remaining}.");
    }
}