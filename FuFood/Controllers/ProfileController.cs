using System.Security.Claims;
using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class ProfileController(UserRepository userRepository) : Controller
{
    [HttpGet("/api/v1/profile")]
    public async Task<IActionResult> Show()
    {
        // AccessTokenHandler 產生了 AuthenticationTicket,要從中拿取 user id
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await userRepository.GetUserById(Guid.Parse(userId!));
        return Ok(new
        {
            Data = user
        });
    }
}