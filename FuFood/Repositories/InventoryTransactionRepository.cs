using FuFood.Data;
using FuFood.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class InventoryTransactionRepository(AppDbContext dbContext)
{
    // Creates an empty transaction. Does not verify if the user has access to the Refrigerator resource.
    public async Task<InventoryTransaction> CreateUserInventoryTransaction(User user, Refrigerator refrigerator)
    {
        var transaction = new InventoryTransaction
        {
            RefrigeratorId = refrigerator.Id,
            UserId = user.Id,
        };

        dbContext.InventoryTransactions.Add(transaction);
        await dbContext.SaveChangesAsync();
        return transaction;
    }

    // 找該使用者有沒有這筆交易
    public async Task<InventoryTransaction?> GetUserInventoryTransaction(User user, Guid transactionId)
    {
        return await dbContext.InventoryTransactions.Where(u => u.UserId == user.Id)
            .FirstOrDefaultAsync(i => i.Id == transactionId);
    }

    // 入庫清單(簡略版)
    public async Task<List<InventoryTransaction>> GetByRefrigerator(Guid refrigeratorId)
    {
        return await dbContext.InventoryTransactions.Where(t => t.RefrigeratorId == refrigeratorId)
            .OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    // 入庫詳細
    public async Task<InventoryTransaction?> GetDetail(Guid transactionId, Guid userId)
    {
        return await dbContext.InventoryTransactions.Where(t => t.Id == transactionId && t.UserId == userId)
            .Include(t => t.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync();
    }
}