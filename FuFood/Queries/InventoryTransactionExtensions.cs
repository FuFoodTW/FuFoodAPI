using FuFood.Models.Entities;

namespace FuFood.Queries;

public static class InventoryTransactionExtensions
{
    extension(IQueryable<InventoryTransaction> query)
    {
        public IQueryable<InventoryTransaction> Committed()
        {
            return query.Where(t => t.CommittedAt != null);
        }

        public IQueryable<InventoryTransaction> Pending()
        {
            return query.Where(t => t.CommittedAt == null);
        }

        public IQueryable<InventoryTransaction> ForUser(User user)
        {
            return query.Where(t => t.UserId == user.Id);
        }
    }
}