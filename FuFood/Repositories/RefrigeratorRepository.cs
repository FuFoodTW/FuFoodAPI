using FuFood.Data;
using FuFood.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class RefrigeratorRepository(AppDbContext dbContext)
{
    public async Task<IEnumerable<Refrigerator>> ListUserRefrigerators(User user)
    {
        return await dbContext.Refrigerators.Where(r => r.CreatedById == user.Id).ToListAsync();
    }

    public async Task<Refrigerator?> GetUserRefrigeratorById(User user, Guid id)
    {
        return await dbContext.Refrigerators.Where(r => r.CreatedById == user.Id).FirstOrDefaultAsync(r => r.Id == id);
    }
}