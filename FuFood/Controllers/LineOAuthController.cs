using System.Security.Cryptography;
using FuFood.Repositories;
using FuFood.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

[ApiController]
[AllowAnonymous]
public class LineOAuthController(
    LineOAuthService lineService,
    UserRepository userRepository,
    JwtService jwtService,
    RefrigeratorInvitationRepository invitationRepository,
    RefrigeratorMembershipService membershipService,
    IHostEnvironment env)
    : Controller
{
    private const string StateCookieName = "oauth_state";
    private const string RedirectUrlCookieName = "oauth_redirect_url";
    private const string DefaultRedirectUrl = "https://fufood.jocelynh.me";
    private const string InvitationTokenCookieName = "oauth_invitation_token";

    // Init() 工作 1.產生隨機值 2.設定 cookie 3.導向 line 登入的 URL
    [HttpGet("/oauth/line/init")]
    public IActionResult Init([FromQuery(Name = "ref")] string? redirectTo,
        [FromQuery(Name = "invite")] string? invitationToken)
    {
        var state = GenerateState();
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            MaxAge = TimeSpan.FromMinutes(10),
            IsEssential = true
        };
        Response.Cookies.Append(StateCookieName, state, cookieOptions);

        if (!string.IsNullOrEmpty(redirectTo))
        {
            Response.Cookies.Append(RedirectUrlCookieName, redirectTo, cookieOptions);
        }

        // 儲存 invitation token
        if (!string.IsNullOrEmpty(invitationToken))
        {
            Response.Cookies.Append(InvitationTokenCookieName, invitationToken, cookieOptions);
        }

        var url = lineService.GetAuthorizationUrl(state);
        return Redirect(url);
    }

    [HttpGet("/oauth/line/callback")]
    public async Task<IActionResult> Callback(string code, string state)
    {
        Request.Cookies.TryGetValue(StateCookieName, out var cookieState);
        if (string.IsNullOrEmpty(cookieState) || state != cookieState)
        {
            return BadRequest();
        }

        Request.Cookies.TryGetValue(RedirectUrlCookieName, out var redirectUrl);

        var response = await lineService.IssueAccessToken(code);
        var claims = lineService.DecodeIdToken(response.IdToken);
        var user = await userRepository.FindOrCreateUserFromLineIdTokenClaims(claims);

        var accessToken = jwtService.IssueAccessTokenForUser(user);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            MaxAge = TimeSpan.FromDays(1),
            Domain = env.IsProduction() ? "fufood.jocelynh.me" : null
        };

        Response.Cookies.Append(Constants.AccessTokenCookieName, accessToken, cookieOptions);
        Response.Cookies.Delete(StateCookieName);
        Response.Cookies.Delete(RedirectUrlCookieName);

        // 自動加入群組（有 invitation token 的話）
        if (Request.Cookies.TryGetValue(InvitationTokenCookieName, out var invToken) && !string.IsNullOrEmpty(invToken))
        {
            var invitation = await invitationRepository.GetInvitationByToken(invToken);
            if (invitation != null)
            {
                await membershipService.CreateFromInvitation(user, invitation);
            }

            Response.Cookies.Delete(InvitationTokenCookieName);
        }

        return Redirect(redirectUrl ?? DefaultRedirectUrl);
    }

    private static string GenerateState()
    {
        var bytes = RandomNumberGenerator.GetBytes(4);
        return Convert.ToHexStringLower(bytes);
    }
}