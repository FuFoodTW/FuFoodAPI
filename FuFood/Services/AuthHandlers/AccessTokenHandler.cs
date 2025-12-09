using System.Security.Claims;
using System.Text.Encodings.Web;
using FuFood.Models;
using FuFood.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace FuFood.Services.AuthHandlers;

public class AccessTokenHandlerOptions : AuthenticationSchemeOptions
{
}

public class AccessTokenHandler(
    IOptionsMonitor<AccessTokenHandlerOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AccessTokenHandlerOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var accessToken = GetAccessTokenFromCookies(Request);
        if (accessToken == null) return AuthenticateResult.Fail("Cookie not found.");

        var services = Request.HttpContext.RequestServices;
        var jwtService = services.GetRequiredService<JwtService>();
        var userRepository = services.GetRequiredService<UserRepository>();
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
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while fetching user data");
        }

        return AuthenticateResult.Fail("Failed to fetch user by access token.");
    }

    private static string? GetAccessTokenFromCookies(HttpRequest request)
    {
        request.Cookies.TryGetValue("access_token", out var token);
        return token;
    }

    private AuthenticationTicket BuildTicketForUser(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name)
        };
        var identity = new ClaimsIdentity(claims, nameof(AccessTokenHandler));
        return new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
    }
}