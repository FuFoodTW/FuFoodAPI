using System.Security.Cryptography;
using FuFood.Data;
using FuFood.Models.Entities;

namespace FuFood.Tests;

public class TestFactory(AppDbContext db)
{
    public async Task<User> CreateUser(Action<User>? action = null)
    {
        var user = new User
        {
            Name = "ABC",
            LineId = GenerateLineId(),
        };
        action?.Invoke(user);

        db.Add(user);
        await db.SaveChangesAsync();

        return user;
    }

    public async Task<Refrigerator> CreateRefrigerator(User user, Action<Refrigerator>? action = null)
    {
        var refrigerator = new Refrigerator
        {
            OwnerId = user.Id,
            Name = "哇欸冰箱",
        };
        action?.Invoke(refrigerator);

        db.Add(refrigerator);
        db.Add(new RefrigeratorMembership
        {
            MemberId = user.Id,
            RefrigeratorId = refrigerator.Id,
        });
        await db.SaveChangesAsync();

        return refrigerator;
    }

    private string GenerateLineId()
    {
        var randomness = RandomNumberGenerator.GetHexString(10);
        return $"U{randomness}";
    }
}