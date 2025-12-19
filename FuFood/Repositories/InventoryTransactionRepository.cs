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
}