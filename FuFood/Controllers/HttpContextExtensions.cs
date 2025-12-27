using System.Security.Claims;
using FuFood.Models.Entities;
using FuFood.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FuFood.Controllers;

public static class HttpContextExtensions
{
    public static Guid? CurrentUserId(this HttpContext context)
    {
        var maybeUuid = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(maybeUuid))
        {
            return null;
        }

        return Guid.Parse(maybeUuid);
    }

    public static async Task<User> GetCurrentUser(this HttpContext context)
    {
        var userRepository = context.RequestServices.GetRequiredService<UserRepository>();
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException("User ID not present in HttpContext");
        }

        var user = await userRepository.GetUserByIdAsNoTracking(Guid.Parse(userId));
        return user!;
    }
}