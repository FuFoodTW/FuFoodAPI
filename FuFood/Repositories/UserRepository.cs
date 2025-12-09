using FuFood.Data;
using FuFood.Models;
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
        await db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserById(Guid id)
    {
        return await db.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
}