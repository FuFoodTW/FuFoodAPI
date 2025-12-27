using FuFood.Models.Entities;
using FuFood.Repositories;

namespace FuFood.Services;

public class CommitTransactionService(InventoryTransactionRepository inventoryTransactionRepository)
{
    public async Task<InventoryTransaction> CommitTransaction(User user, Guid transactionId)
    {
        throw new NotImplementedException();
    }
}