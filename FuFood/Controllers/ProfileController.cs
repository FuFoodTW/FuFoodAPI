using FuFood.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class ProfileController : Controller
{
    [HttpGet("/api/v1/profile")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProfile()
    {
        var user = await HttpContext.GetCurrentUserAsync();

        return Ok(user);
    }
}