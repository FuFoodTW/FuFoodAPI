using System.Net;
using FuFood.Models.Requests;
using FuFood.Repositories;
using FuFood.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class RefrigeratorMembershipController(
    RefrigeratorInvitationRepository invitationRepository,
    RefrigeratorMembershipService membershipService,
    RefrigeratorRepository refrigeratorRepository) : Controller
{
    /// <summary>
    /// Creates a `RefrigeratorMembership` with the current user using a valid invitation token.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/api/v1/refrigerator_memberships")]
    public async Task<IActionResult> Create([FromBody] RefrigeratorMembershipCreateRequest request)
    {
        var user = await HttpContext.GetCurrentUser();
        var invitation = await invitationRepository.GetInvitationByToken(request.InvitationToken);
        if (invitation == null)
        {
            return UnprocessableEntity("Invitation token is invalid or expired");
        }

        try
        {
            var membership = await membershipService.CreateFromInvitation(user, invitation);
            return Ok(new
            {
                Data = membership
            });
        }
        catch (Exception e)
        {
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpDelete("/api/v1/refrigerator/{refrigeratorId:guid}/memberships/{memberId:guid}")]
    public async Task<IActionResult> Delete(Guid refrigeratorId, Guid memberId)
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerator = await refrigeratorRepository.GetOwnedRefrigeratorById(user, refrigeratorId);
        if (refrigerator == null)
        {
            return Forbid();
        }

        if (memberId == user.Id)
        {
            return UnprocessableEntity("You cannot remove yourself from a refrigerator you own.");
        }

        await membershipService.DeleteMembership(refrigeratorId, memberId);

        return NoContent();
    }
}