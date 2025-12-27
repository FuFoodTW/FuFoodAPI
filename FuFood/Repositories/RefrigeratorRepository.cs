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
        return await dbContext.Refrigerators.Where(r => r.CreatedById == user.Id).ToListAsync();
    }

    // 列出冰箱建立者的某個冰箱
    public async Task<Refrigerator?> GetUserRefrigeratorById(User user, Guid id)
    {
        return await dbContext.Refrigerators.Where(r => r.CreatedById == user.Id).FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Refrigerator?> GetByIdAsync(Guid id)
    {
        return await dbContext.Refrigerators.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Refrigerator?> GetWithMembersByIdAsync(Guid id)
    {
        return await dbContext.Refrigerators
            .Include(r => r.Members)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Refrigerator?> GetByQrCodeAsync(string qrCode)
    {
        return await dbContext.Refrigerators.FirstOrDefaultAsync(r => r.QrCode == qrCode);
    }

    public async Task<Refrigerator> Create(User user, Refrigerator refrigerator)
    {
        refrigerator.CreatedById = user.Id;
        dbContext.Refrigerators.Add(refrigerator);
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
            await dbContext.Refrigerators.FirstOrDefaultAsync(r => r.Id == id && r.CreatedById == user.Id);

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
        return await dbContext.RefrigeratorMembers
            .AnyAsync(rm => rm.RefrigeratorId == refrigeratorId && rm.MemberId == userId);
    }

    public async Task<int> GetMemberCountAsync(Guid refrigeratorId)
    {
        return await dbContext.RefrigeratorMembers
            .CountAsync(rm => rm.RefrigeratorId == refrigeratorId);
    }

    public async Task AddMemberAsync(Guid refrigeratorId, Guid userId)
    {
        var member = new RefrigeratorMember
        {
            RefrigeratorId = refrigeratorId,
            MemberId = userId,
            JoinedAt = DateTime.UtcNow
        };
        dbContext.RefrigeratorMembers.Add(member);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> RemoveMemberAsync(Guid refrigeratorId, Guid userId)
    {
        var member = await dbContext.RefrigeratorMembers
            .FirstOrDefaultAsync(rm => rm.RefrigeratorId == refrigeratorId && rm.MemberId == userId);

        if (member == null) return false;

        dbContext.RefrigeratorMembers.Remove(member);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task UpdateOwnershipAsync(Guid refrigeratorId, Guid newOwnerId)
    {
        var refrigerator = await dbContext.Refrigerators.FindAsync(refrigeratorId);
        if (refrigerator != null)
        {
            refrigerator.CreatedById = newOwnerId;
            refrigerator.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
        }
    }
}