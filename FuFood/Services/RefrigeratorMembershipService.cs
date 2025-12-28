using FuFood.Data;
using FuFood.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Services;

public class RefrigeratorMembershipService(AppDbContext dbContext)
{
    private const int FreeSubscriptionLimit = 3;
    private const int ProSubscriptionLimit = 5;

    public async Task<RefrigeratorMember> CreateFromInvitation(User user, RefrigeratorInvitation invitation)
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

        var membership = new RefrigeratorMember
        {
            RefrigeratorId = invitation.RefrigeratorId,
            MemberId = user.Id,
        };

        dbContext.Add(membership);
        await dbContext.SaveChangesAsync();
        return membership;
    }

    private async Task<bool> IsMemberOrOwner(User user, Guid refrigeratorId)
    {
        return await dbContext.Database
            .SqlQueryRaw<bool>(
                """
                select exists
                    (select from "Refrigerators" where "Id" = {0} and "OwnerId" = {1})
                or exists
                    (select from "RefrigeratorMembers" where "RefrigeratorId" = {0} and "MemberId" = {1});
                """, refrigeratorId, user.Id)
            .FirstOrDefaultAsync();
    }

    private async Task<int> CalculateRemainingMemberCapacity(Guid refrigeratorId)
    {
        return await dbContext.Database
            .SqlQueryRaw<int>(
                """
                select
                    -- base capacity based on the user's subscription tier
                    case u."SubscriptionTier"
                        when 'free' then {0}
                        when 'pro' then {1}
                    end
                        -- minus existing member count
                        - count(m."Id")
                        -- minus the owner
                        - 1
                from "Refrigerators" r
                left join "RefrigeratorMembers" m on m."RefrigeratorId" = r."Id"
                join "Users" u on r."OwnerId" = u."Id"
                where r."Id" = {2}
                group by u."SubscriptionTier";
                """, FreeSubscriptionLimit, ProSubscriptionLimit, refrigeratorId)
            .FirstOrDefaultAsync();
    }
}