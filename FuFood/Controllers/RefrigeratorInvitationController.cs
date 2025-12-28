using FuFood.Repositories;
using FuFood.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class RefrigeratorInvitationController(
    RefrigeratorRepository refrigeratorRepository,
    RefrigeratorInvitationRepository invitationRepository,
    RefrigeratorInvitationService service) : Controller
{
    [HttpPost("/api/v1/refrigerators/{refrigeratorId:guid}/invitations")]
    public async Task<IActionResult> Create(Guid refrigeratorId)
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, refrigeratorId);
        if (refrigerator == null)
        {
            return NotFound();
        }

        var invitation = await service.CreateRefrigeratorInvitation(user, refrigerator);

        return Ok(new
        {
            Data = invitation
        });
    }

    [HttpGet("/api/v1/invitations/{token}")]
    public async Task<IActionResult> Show(string token)
    {
        var invitation = await invitationRepository.GetInvitationByToken(token);
        if (invitation == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            Data = invitation
        });
    }
}