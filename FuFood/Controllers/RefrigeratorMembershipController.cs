using FuFood.Models.Requests;
using FuFood.Repositories;
using FuFood.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class RefrigeratorMembershipController(
    RefrigeratorInvitationRepository invitationRepository,
    RefrigeratorMembershipService membershipService) : Controller
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
}