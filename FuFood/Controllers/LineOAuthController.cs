using System.Security.Cryptography;
using FuFood.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

[ApiController]
public class LineOAuthController(LineOAuthService service) : Controller
{
    private const string CookieName = "oauth_state";

    [HttpGet("/oauth/line/init")]
    public IActionResult Init()
    {
        var state = GenerateState();
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = HttpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            MaxAge = TimeSpan.FromMinutes(10),
            IsEssential = true
        };
        Response.Cookies.Append(CookieName, state, cookieOptions);
        var url = service.GetAuthorizationUrl(state);
        return Redirect(url);
    }

    private string GenerateState()
    {
        var bytes = RandomNumberGenerator.GetBytes(4);
        return Convert.ToHexStringLower(bytes);
    }
}