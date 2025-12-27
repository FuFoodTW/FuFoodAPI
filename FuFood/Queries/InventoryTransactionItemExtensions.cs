using FuFood.Models.Entities;
using FuFood.Models.Enums;

namespace FuFood.Queries;

public static class InventoryTransactionItemExtensions
{
    extension(IQueryable<InventoryTransactionItem> query)
    {
        public IQueryable<InventoryTransactionItem> Finalized()
        {
            return query.Where(i => i.InventoryTransaction!.FinalizedAt != null);
        }

        public IQueryable<InventoryTransactionItem> Parents()
        {
            return query.Where(i => i.ParentId == null);
        }

        public IQueryable<InventoryTransactionItem> Consumers()
        {
            return query.Where(i => i.ParentId != null);
        }

        public IQueryable<InventoryTransactionItem> ForRefrigerator(Guid refrigeratorId)
        {
            return query.Where(i => i.InventoryTransaction!.RefrigeratorId == refrigeratorId);
        }

        public IQueryable<InventoryTransactionItem> ForTransaction(Guid transactionId)
        {
            return query.Where(i => i.InventoryTransactionId == transactionId);
        }

        public IQueryable<InventoryTransactionItem> WithCategory(ProductCategory filter)
        {
            return query.Where(i => i.Product != null && (i.Product.Categories & filter) == filter);
        }

        public IQueryable<InventoryTransactionItem> Consumable()
        {
            return query
                .Finalized()
                .Parents()
                .Where(i => i.FullyConsumedAt == null);
        }
    }
}