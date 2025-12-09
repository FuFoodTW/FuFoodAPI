using System.Security.Claims;
using FuFood.Models;
using FuFood.Repositories;

namespace FuFood.Services;

public static class HttpContextExtensions
{
    extension(HttpContext context)
    {
        public async Task<User?> GetCurrentUserAsync()
        {
            var repo = context.RequestServices.GetRequiredService<UserRepository>();
            var userId = context.GetUserId();
            if (userId == null) return null;
            return await repo.GetUserById(userId.Value);
        }

        public Guid? GetUserId()
        {
            var id = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var valid = Guid.TryParse(id, out var guid);
            return valid ? guid : null;
        }
    }
}