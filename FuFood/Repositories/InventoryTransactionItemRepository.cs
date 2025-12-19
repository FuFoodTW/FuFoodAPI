using FuFood.Data;
using FuFood.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class InventoryTransactionItemRepository(AppDbContext dbContext)
{
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
}