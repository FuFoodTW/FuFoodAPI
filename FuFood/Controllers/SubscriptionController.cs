using FuFood.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class SubscriptionController(SubscriptionService service) : Controller
{
    [HttpPost("/api/v1/subscription")]
    public async Task<IActionResult> Create()
    {
        var user = await HttpContext.GetCurrentUser();
        await service.ExtendSubscription(user);
        return NoContent();
    }

    [HttpDelete("/api/v1/subscription")]
    public async Task<IActionResult> Delete()
    {
        var user = await HttpContext.GetCurrentUser();
        await service.CancelSubscription(user);
        return NoContent();
    }
}