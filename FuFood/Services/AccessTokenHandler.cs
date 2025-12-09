using System.Security.Claims;
using System.Text.Encodings.Web;
using FuFood.Models;
using FuFood.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FuFood.Services;

// 處理每個請求(cookie 和資料庫)的驗證
public class AccessTokenHandler(
    IOptionsMonitor<AccessTokenHandler.AccessTokenHandlerOptions> options,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder)
    : AuthenticationHandler<AccessTokenHandler.AccessTokenHandlerOptions>(options, loggerFactory, encoder)
{
    public class AccessTokenHandlerOptions : AuthenticationSchemeOptions
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Request.Cookies.TryGetValue("access_token", out var accessToken);
        if (string.IsNullOrEmpty(accessToken))
        {
            return AuthenticateResult.Fail("no cookie");
        }

        var services = Request.HttpContext.RequestServices; // 會呼叫 Program.cs 裡的所有 services, 為了方便拿取 JwtService
        var jwtService = services.GetRequiredService<JwtService>(); // 主動 DI
        var repo = services.GetRequiredService<UserRepository>();
        try
        {
            var claims = jwtService.DecodeAccessToken(accessToken);
            var user = await repo.GetUserById(claims.Subject);
            if (user != null)
            {
                var ticket = BuildTicketForUser(user);
                return AuthenticateResult.Success(ticket);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return AuthenticateResult.Fail("用戶驗證錯誤");
    }

    private AuthenticationTicket BuildTicketForUser(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        var identity = new ClaimsIdentity(claims, nameof(AccessTokenHandler));
        return new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
    }
}