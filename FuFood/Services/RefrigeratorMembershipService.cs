using FuFood.Data;
using FuFood.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Services;

public class RefrigeratorMembershipService(AppDbContext dbContext)
{
    private const int FreeSubscriptionLimit = 5;
    private const int ProSubscriptionLimit = 10;

    public async Task<RefrigeratorMembership> CreateFromInvitation(User user, RefrigeratorInvitation invitation)
    {
        var freeCapacity = await CalculateRemainingMemberCapacity(invitation.RefrigeratorId);
        if (freeCapacity <= 0)
        {
            throw new InvalidOperationException("No membership slots left for this refrigerator.");
        }

        var alreadyAssociated = await IsMemberOrOwner(user, invitation.RefrigeratorId);
        if (alreadyAssociated)
        {
            throw new InvalidOperationException("You already have access to this refrigerator");
        }

        var membership = new RefrigeratorMembership
        {
            RefrigeratorId = invitation.RefrigeratorId,
            MemberId = user.Id,
        };

        dbContext.Add(membership);
        dbContext.Remove(invitation);
        await dbContext.SaveChangesAsync();
        return membership;
    }

    private async Task<bool> IsMemberOrOwner(User user, Guid refrigeratorId)
    {
        return await dbContext.Database
            .SqlQueryRaw<bool>(
                """
                select (exists
                    (select from "Refrigerators" where "Id" = {0} and "OwnerId" = {1})
                or exists
                    (select from "RefrigeratorMemberships" where "RefrigeratorId" = {0} and "MemberId" = {1})) AS "Value"
                """, refrigeratorId, user.Id)
            .FirstOrDefaultAsync();
    }

    private async Task<int> CalculateRemainingMemberCapacity(Guid refrigeratorId)
    {
        return await dbContext.Database
            .SqlQueryRaw<int>(
                """
                select (case u."SubscriptionTier"
                        when 'free' then {0}
                        when 'pro' then {1}
                    end - count(m."Id")) AS "Value"
                from "Refrigerators" r
                left join "RefrigeratorMemberships" m on m."RefrigeratorId" = r."Id"
                join "Users" u on r."OwnerId" = u."Id"
                where r."Id" = {2}
                group by u."SubscriptionTier"
                """, FreeSubscriptionLimit, ProSubscriptionLimit, refrigeratorId)
            .FirstOrDefaultAsync();
    }

    public async Task DeleteMembership(Guid refrigeratorId, Guid memberId)
    {
        await dbContext.RefrigeratorMemberships
            .Where(m => m.MemberId == memberId)
            .Where(m => m.RefrigeratorId == refrigeratorId)
            .ExecuteDeleteAsync();
    }
}