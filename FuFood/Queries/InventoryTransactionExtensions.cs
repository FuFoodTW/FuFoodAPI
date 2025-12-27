using FuFood.Models.Entities;

namespace FuFood.Queries;

public static class InventoryTransactionExtensions
{
    extension(IQueryable<InventoryTransaction> query)
    {
        public IQueryable<InventoryTransaction> Finalized()
        {
            return query.Where(t => t.FinalizedAt != null);
        }

        public IQueryable<InventoryTransaction> Drafts()
        {
            return query.Where(t => t.FinalizedAt == null);
        }

        public IQueryable<InventoryTransaction> ForUser(User user)
        {
            return query.Where(t => t.UserId == user.Id);
        }
    }
}