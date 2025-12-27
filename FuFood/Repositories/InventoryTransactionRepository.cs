using FuFood.Data;
using FuFood.Models.Entities;
using FuFood.Queries;
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
            UserId = user.Id
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

    public async Task<InventoryTransaction?> GetConsumableTransaction(User user, Guid transactionId)
    {
        return await dbContext.InventoryTransactions
            .Committed()
            .ForUser(user)
            .FirstOrDefaultAsync(t => t.Id == transactionId);
    }

    public async Task<InventoryTransaction?> GetDraftTransaction(User user, Guid transactionId)
    {
        return await dbContext.InventoryTransactions
            .Pending()
            .ForUser(user)
            .FirstOrDefaultAsync(t => t.Id == transactionId);
    }

    // A transaction can only be finalized if it is a draft (has not been finalized yet)
    // and if it contains any items (we do not accept empty transactions)
    public async Task<bool> HasItems(InventoryTransaction transaction)
    {
        return await dbContext.InventoryTransactionsItems
            .AnyAsync(i => i.InventoryTransactionId == transaction.Id);
    }

    // // Calculate the total amount that the transaction intends to consume
    // public async Task<bool> ValidateRemainingInventory(InventoryTransaction transaction)
    // {
    //     var requestedQuantities = await dbContext.InventoryTransactionsItems
    //         .Consumers()
    //         .ForTransaction(transaction.Id)
    //         .Where()
    //     var consumingItemIds = transaction.Items
    // }

    public async Task<InventoryTransaction> FinalizeTransaction(InventoryTransaction transaction)
    {
        if (transaction.CommittedAt != null)
            throw new InvalidOperationException("The transaction has already been finalized!");

        transaction.CommittedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        return transaction;
    }

    // 入庫詳細
    public async Task<InventoryTransaction?> GetDetail(Guid transactionId, Guid userId)
    {
        return await dbContext.InventoryTransactions.Where(t => t.Id == transactionId && t.UserId == userId)
            .Include(t => t.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync();
    }
}