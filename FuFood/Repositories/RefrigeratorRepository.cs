using System.Runtime.InteropServices.JavaScript;
using FuFood.Data;
using FuFood.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class RefrigeratorRepository(AppDbContext dbContext)
{
    // 列出冰箱建立者的所有冰箱
    public async Task<IEnumerable<Refrigerator>> ListUserRefrigerators(User user)
    {
        return await dbContext.RefrigeratorMemberships
            .Where(rm => rm.MemberId == user.Id)
            .Select(rm => rm.Refrigerator!)
            .OrderBy(r => r!.Id)
            .ToListAsync();
    }

    public async Task<Refrigerator?> GetUserRefrigeratorById(User user, Guid id)
    {
        return await dbContext.RefrigeratorMemberships
            .Where(rm => rm.RefrigeratorId == id)
            .Where(rm => rm.MemberId == user.Id)
            .Select(rm => rm.Refrigerator)
            .FirstOrDefaultAsync();
    }

    public async Task<Refrigerator?> GetOwnedRefrigeratorById(User user, Guid id)
    {
        return await dbContext.Refrigerators
            .Where(r => r.OwnerId == user.Id)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Refrigerator?> GetByIdAsync(Guid id)
    {
        return await dbContext.Refrigerators.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Refrigerator?> GetWithMembersByIdAsync(Guid id)
    {
        return await dbContext.Refrigerators
            .Include(r => r.Memberships)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Refrigerator> Create(User user, Refrigerator refrigerator)
    {
        refrigerator.OwnerId = user.Id;
        dbContext.Refrigerators.Add(refrigerator);
        dbContext.RefrigeratorMemberships.Add(new RefrigeratorMembership
        {
            MemberId = user.Id,
            RefrigeratorId = refrigerator.Id,
        });
        await dbContext.SaveChangesAsync();

        return refrigerator;
    }

    // 只能編輯自己的冰箱
    public async Task<Refrigerator?> Update(User user, Refrigerator refrigerator, string name, string? colour)
    {
        refrigerator.Name = name;
        refrigerator.Colour = colour;
        refrigerator.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        return refrigerator;
    }

    public async Task<bool> Delete(User user, Guid id)
    {
        var refrigerator =
            await dbContext.Refrigerators.FirstOrDefaultAsync(r => r.Id == id && r.OwnerId == user.Id);

        if (refrigerator == null)
        {
            return false;
        }

        dbContext.Refrigerators.Remove(refrigerator);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsMemberAsync(Guid refrigeratorId, Guid userId)
    {
        return await dbContext.RefrigeratorMemberships
            .AnyAsync(rm => rm.RefrigeratorId == refrigeratorId && rm.MemberId == userId);
    }

    public async Task<int> GetMemberCountAsync(Guid refrigeratorId)
    {
        return await dbContext.RefrigeratorMemberships
            .CountAsync(rm => rm.RefrigeratorId == refrigeratorId);
    }

    public async Task AddMemberAsync(Guid refrigeratorId, Guid userId)
    {
        var member = new RefrigeratorMembership
        {
            RefrigeratorId = refrigeratorId,
            MemberId = userId,
            CreatedAt = DateTime.UtcNow
        };
        dbContext.RefrigeratorMemberships.Add(member);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> RemoveMemberAsync(Guid refrigeratorId, Guid userId)
    {
        var member = await dbContext.RefrigeratorMemberships
            .FirstOrDefaultAsync(rm => rm.RefrigeratorId == refrigeratorId && rm.MemberId == userId);

        if (member == null) return false;

        dbContext.RefrigeratorMemberships.Remove(member);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task UpdateOwnershipAsync(Guid refrigeratorId, Guid newOwnerId)
    {
        var refrigerator = await dbContext.Refrigerators.FindAsync(refrigeratorId);
        if (refrigerator != null)
        {
            refrigerator.OwnerId = newOwnerId;
            refrigerator.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }
    }
}