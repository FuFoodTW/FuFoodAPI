using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class SessionController(RevokedAccessTokenRepository repository) : Controller
{
    [HttpDelete("/api/v1/session")]
    public async Task<IActionResult> Delete()
    {
        Request.Cookies.TryGetValue(Constants.AccessTokenCookieName, out var accessToken);
        if (!string.IsNullOrEmpty(accessToken))
        {
            await repository.RevokeAccessToken(accessToken);
        }

        Response.Cookies.Delete(Constants.AccessTokenCookieName);
        return NoContent();
    }
}