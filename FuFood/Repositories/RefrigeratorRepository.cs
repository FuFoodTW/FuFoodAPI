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

    // 取得單一特定會員冰箱
    public async Task<Refrigerator?> GetUserRefrigeratorById(User user, Guid id)
    {
        return await dbContext.RefrigeratorMemberships
            .Where(rm => rm.RefrigeratorId == id)
            .Where(rm => rm.MemberId == user.Id)
            .Select(rm => rm.Refrigerator)
            .FirstOrDefaultAsync();
    }

    // 取得擁有的單一個冰箱
    public async Task<Refrigerator?> GetOwnedRefrigeratorById(User user, Guid id)
    {
        return await dbContext.Refrigerators
            .Where(r => r.OwnerId == user.Id)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    // 取得單一個冰箱
    public async Task<Refrigerator?> GetByIdAsync(Guid id)
    {
        return await dbContext.Refrigerators.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Refrigerator> PreloadAssocs(Refrigerator refrigerator)
    {
        await dbContext.Entry(refrigerator).Collection(r => r.Members).LoadAsync();
        return refrigerator;
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
    public async Task<Refrigerator?> Update(User user, Refrigerator refrigerator, string name)
    {
        refrigerator.Name = name;
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

    public async Task LeaveRefrigeratorById(RefrigeratorMembership membership)
    {
        dbContext.RefrigeratorMemberships.Remove(membership);
        await dbContext.SaveChangesAsync();
    }
}