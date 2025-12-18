using FuFood.Data;
using FuFood.Models;
using FuFood.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class UserRepository(AppDbContext db)
{
    public async Task<User> FindOrCreateUserFromLineIdTokenClaims(LineIdTokenClaims claims)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.LineId == claims.Subject);
        if (user != null)
        {
            return user;
        }

        user = new User
        {
            LineId = claims.Subject,
            Name = claims.Name,
            ProfilePictureUrl = claims.Picture
        };
        db.Users.Add(user);

        // 建立預設冰箱
        var defaultRefrigerator = new Refrigerator
        {
            Name = "我的冰箱",
            CreatedById = user.Id
        };

        db.Refrigerators.Add(defaultRefrigerator);

        await db.SaveChangesAsync();

        return user;
    }

    public async Task<User?> GetUserById(Guid id)
    {
        return await db.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
}