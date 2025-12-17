using System.Security.Claims;
using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class ProfileController(UserRepository userRepository) : Controller
{
    [HttpGet("/api/v1/profile")]
    public async Task<IActionResult> Show()
    {
        var user = await HttpContext.GetCurrentUser();
        return Ok(new
        {
            Data = user
        });
    }
}