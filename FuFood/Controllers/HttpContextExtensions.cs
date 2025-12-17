using System.Security.Claims;
using FuFood.Models.Entities;
using FuFood.Repositories;

namespace FuFood.Controllers;

public static class HttpContextExtensions
{
    extension(HttpContext context)
    {
        public Guid? CurrentUserId()
        {
            var maybeUuid = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(maybeUuid))
            {
                return null;
            }

            return Guid.Parse(maybeUuid);
        }

        public async Task<User?> GetCurrentUser()
        {
            var userRepository = context.RequestServices.GetRequiredService<UserRepository>();
            var userId = context.CurrentUserId();
            if (userId == null)
            {
                return null;
            }

            return await userRepository.GetUserById(userId.Value);
        }
    }
}