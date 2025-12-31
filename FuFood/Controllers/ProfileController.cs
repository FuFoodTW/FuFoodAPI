using System.Security.Claims;
using FuFood.Models.Requests;
using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class ProfileController(ProfileRepository profileRepository) : Controller
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

    [HttpPut("/api/v1/profile/{userId:guid}")]
    public async Task<IActionResult> Update(Guid userId, [FromBody] UpsertProfileRequest upsertProfileRequest)
    {
        var user = await HttpContext.GetCurrentUser();

        var newUser = await profileRepository.UpdateProfile(user, upsertProfileRequest);

        return Ok(new
        {
            Data = newUser
        });
    }
}