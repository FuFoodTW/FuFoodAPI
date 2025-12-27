using FuFood.Data;
using FuFood.Models.Entities;
using FuFood.Queries;
using FuFood.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Services;

public class CommitTransactionService(
    AppDbContext dbContext,
    InventoryQueryRepository inventoryRepository)
{
    private enum RemainingInventory
    {
        Available = 1,
        FullyConsumed = 0,
        Overconsumed = -1
    }

    public async Task<InventoryTransaction> CommitTransaction(User user, InventoryTransaction transaction)
    {
        if (!await HasItems(transaction))
        {
            throw new InvalidOperationException("Only transactions with items can be committed");
        }

        var inventoryStates = await CheckRemainingInventoryStates(transaction);
        var overconsumed = inventoryStates.Where(i => i.Value == RemainingInventory.Overconsumed).Select(i => i.Key)
            .ToList();

        if (overconsumed.Count != 0)
        {
            var joined = string.Join(", ", overconsumed);
            throw new InvalidOperationException(
                $"The transaction would result in the overconsumption of the items: {joined}.");
        }

        var now = DateTime.UtcNow;

        // Manually begin a database transaction to handle an `ExecuteUpdateAsync`
        await dbContext.Database.BeginTransactionAsync();

        await MarkFullyConsumedItems(inventoryStates, now);

        transaction.CommittedAt = now;
        transaction.UpdatedAt = now;

        await dbContext.SaveChangesAsync();
        await dbContext.Database.CommitTransactionAsync();

        return transaction;
    }

    private async Task<Dictionary<Guid, RemainingInventory>> CheckRemainingInventoryStates(
        InventoryTransaction transaction)
    {
        var requested = await GetRequestedConsumptionQuantities(transaction);
        if (requested.Count == 0)
        {
            return [];
        }

        var remaining = await inventoryRepository.GetRemainingInventoryForItems(requested.Keys);

        var stocks = new Dictionary<Guid, RemainingInventory>();

        foreach (var (parentId, requestedQuantity) in requested)
        {
            // If there is no value provided in stocks, the item is fully consumed; treat this as 0
            var remainingQuantity = remaining.GetValueOrDefault(parentId, 0);

            // Returns -1/0/1 from negative/zero/positive value
            var sign = Math.Sign(remainingQuantity.CompareTo(requestedQuantity));

            // If actual is greater than the requested amount, we return `Partial`
            // If it's less than the requested amount, this will return -1, or `Overconsumed`
            // If they are equal, it's fully consumed.
            stocks.Add(parentId, (RemainingInventory)sign);
        }

        return stocks;
    }

    // A transaction can only be finalized if it is a draft (has not been finalized yet)
    // and if it contains any items (we do not accept empty transactions)
    private async Task<bool> HasItems(InventoryTransaction transaction)
    {
        return await dbContext.InventoryTransactionsItems
            .AnyAsync(i => i.InventoryTransactionId == transaction.Id);
    }

    private async Task<Dictionary<Guid, decimal>> GetRequestedConsumptionQuantities(InventoryTransaction transaction)
    {
        return await dbContext.InventoryTransactionsItems
            .ForTransaction(transaction)
            .Consumers()
            .Select(i => new { i.ParentId, i.Quantity })
            .ToDictionaryAsync(x => x.ParentId!.Value, x => Math.Abs(x.Quantity));
    }

    private async Task MarkFullyConsumedItems(Dictionary<Guid, RemainingInventory> map, DateTime now)
    {
        var ids = map.Where(i => i.Value == RemainingInventory.FullyConsumed).Select(i => i.Key).ToList();
        if (ids.Count == 0) return;

        await dbContext.InventoryTransactionsItems
            .Where(i => ids.Contains(i.Id))
            .ExecuteUpdateAsync(s =>
                s.SetProperty(i => i.FullyConsumedAt, now)
                    .SetProperty(i => i.UpdatedAt, now));
    }
}