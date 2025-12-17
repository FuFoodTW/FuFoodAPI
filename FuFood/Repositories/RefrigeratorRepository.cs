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

    public async Task<Refrigerator> Create(User user, Refrigerator refrigerator)
    {
        refrigerator.CreatedById = user.Id;
        refrigerator.CreatedAt = DateTime.UtcNow;
        refrigerator.UpdatedAt = DateTime.UtcNow;

        dbContext.Refrigerators.Add(refrigerator);
        await dbContext.SaveChangesAsync();

        return refrigerator;
    }

    // 只能編輯自己的冰箱
    public async Task<bool> Update(User user, Guid id, string name, string? colour)
    {
        var refrigerator =
            await dbContext.Refrigerators.FirstOrDefaultAsync(r => r.Id == id && r.CreatedById == user.Id);

        if (refrigerator == null)
        {
            return false;
        }

        refrigerator.Name = name;
        refrigerator.Colour = colour;
        refrigerator.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        return true;
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
}