using FuFood.Data;
using FuFood.Models.Entities;

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
}