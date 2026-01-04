using FuFood.Data;
using FuFood.Models.Entities;
using FuFood.Models.Enums;

namespace FuFood.Services;

public class SubscriptionService(AppDbContext dbContext)
{
    public async Task<User> ExtendSubscription(User user)
    {
        // If their subscription is not currently active, it starts from now,
        // otherwise we add time at the end
        var beginningFrom = (user.SubscriptionValidUntil == null || user.SubscriptionValidUntil < DateTime.UtcNow)
            ? DateTime.UtcNow
            : user.SubscriptionValidUntil.Value;

        user.SubscriptionValidUntil = beginningFrom.AddMonths(1);
        await dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User> CancelSubscription(User user)
    {
        user.SubscriptionValidUntil = null;
        await dbContext.SaveChangesAsync();
        return user;
    }
}