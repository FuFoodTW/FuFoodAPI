using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class SessionController : Controller
{
    [HttpDelete("/api/v1/session")]
    public async Task<IActionResult> Delete()
    {
        Response.Cookies.Delete(Constants.AccessTokenCookieName);
        return NoContent();
    }
}