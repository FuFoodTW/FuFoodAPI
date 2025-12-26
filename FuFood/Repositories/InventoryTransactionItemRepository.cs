using FuFood.Data;
using FuFood.Models.Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class InventoryTransactionItemRepository(AppDbContext dbContext)
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

    public async Task Delete(InventoryTransactionItem item)
    {
        dbContext.InventoryTransactionsItems.Remove(item);
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 計算庫存剩餘數量
    /// </summary>
    /// <param name="inventoryItemId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    // 計算庫存剩餘數量
    public async Task<decimal> GetRemainingQuantity(Guid inventoryItemId)
    {
        // 入庫(尚未被消耗)
        var item = await dbContext.InventoryTransactionsItems.FirstOrDefaultAsync(i =>
            i.Id == inventoryItemId && i.ParentId == null);

        if (item == null) throw new Exception("Invalid inventory item");

        var consumed = await dbContext.InventoryTransactionsItems
            .Where(i => i.ParentId == inventoryItemId)
            .SumAsync(i => i.Quantity);

        return item.Quantity - consumed; //剩餘數量=入庫-所有子 item
    }

    // 建立消耗 item
    public async Task<InventoryTransactionItem> Consume(InventoryTransaction transaction, Guid inventoryItemId,
        decimal quantity)
    {
        var parentItem = await dbContext.InventoryTransactionsItems.Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == inventoryItemId && i.ParentId == null);

        if (parentItem == null) throw new Exception("Inventory item not found");

        var consumed = await dbContext.InventoryTransactionsItems
            .Where(i => i.ParentId == inventoryItemId)
            .SumAsync(i => i.Quantity);

        var remaining = parentItem.Quantity - consumed;
        if (quantity <= 0 || quantity > remaining) throw new Exception("Insufficient inventory");

        var consumeItem = new InventoryTransactionItem
        {
            ParentId = parentItem.Id,
            ProductId = parentItem.ProductId,
            Quantity = -quantity,
            ExpirationDate = parentItem.ExpirationDate,
            InventoryTransactionId = transaction.Id
        };

        dbContext.InventoryTransactionsItems.Add(consumeItem);
        await dbContext.SaveChangesAsync();

        return consumeItem;
    }
}