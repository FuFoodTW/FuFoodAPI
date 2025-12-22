using System.Security.Claims;
using System.Text.Encodings.Web;
using FuFood.Controllers;
using FuFood.Models;
using FuFood.Models.Entities;
using FuFood.Repositories;
using JWT.Exceptions;
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

    // asp.net 要求的驗證策略, 這函式在每次有請求的時候都會被呼叫,檢驗一次 access token 及使用者是否還存在
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var accessToken = GetAccessTokenFromCookie() ?? GetAccessTokenFromBearer();

        // 檢查請求裡的 cookie 有沒有我要的值
        if (string.IsNullOrEmpty(accessToken))
        {
            return AuthenticateResult.Fail("no cookie");
        }

        var services = Request.HttpContext.RequestServices; // 會呼叫 Program.cs 裡的所有 services, 為了方便拿取 JwtService
        var jwtService = services.GetRequiredService<JwtService>(); // 主動 DI
        var userRepository = services.GetRequiredService<UserRepository>();
        var tokenRepository = services.GetRequiredService<RevokedAccessTokenRepository>();

        if (await tokenRepository.IsTokenRevoked(accessToken))
        {
            return AuthenticateResult.Fail("Access token has been revoked");
        }

        try
        {
            var claims = jwtService.DecodeAccessToken(accessToken);
            var user = await userRepository.GetUserById(claims.Subject);
            if (user != null)
            {
                var ticket = BuildTicketForUser(user);
                return AuthenticateResult.Success(ticket);
            }
        }
        catch (TokenExpiredException e)
        {
            return AuthenticateResult.Fail("Access token expired");
        }
        catch (SignatureVerificationException e)
        {
            Console.WriteLine(e);
        }

        return AuthenticateResult.Fail("Invalid access token");
    }

    private string? GetAccessTokenFromCookie()
    {
        Request.Cookies.TryGetValue(Constants.AccessTokenCookieName, out var accessToken);
        return accessToken;
    }

    private string? GetAccessTokenFromBearer()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var header))
        {
            return null;
        }

        var headerValue = header.ToString();
        if (!headerValue.StartsWith("Bearer "))
        {
            return null;
        }

        return headerValue.Replace("Bearer ", "");
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