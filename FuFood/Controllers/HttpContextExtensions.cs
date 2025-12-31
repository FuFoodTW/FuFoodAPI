using System.Security.Claims;
using FuFood.Models.Entities;
using FuFood.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

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

        public async Task<User> GetCurrentUser()
        {
            var userId = context.CurrentUserId();
            if (userId == null)
            {
                throw new InvalidOperationException("User ID not present in HttpContext");
            }

            var userRepository = context.RequestServices.GetRequiredService<UserRepository>();
            var user = await userRepository.GetUserById(userId.Value);
            return user!;
        }
    }
}