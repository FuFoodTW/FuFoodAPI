using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace FuFood.Services;

public class LineOAuthService(HttpClient client, IOptions<LineOAuthOptions> options)
{
    private readonly LineOAuthOptions _options = options.Value;
    public const string AuthorizeBaseUrl = "https://access.line.me/oauth2/v2.1/authorize";
    public const string IssueAccessTokenUrl = "https://api.line.me/oauth2/v2.1/token";

    public string GetAuthorizationUrl(string state)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["response_type"] = "code",
            ["client_id"] = _options.ClientId,
            ["redirect_uri"] = _options.CallbackUrl,
            ["state"] = state,
            ["scope"] = "profile openid"
        };
        return QueryHelpers.AddQueryString(AuthorizeBaseUrl, queryParams!);
    }

    public async Task<string> IssueAccessToken(string code)
    {
        var body = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = _options.CallbackUrl,
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret
        };
        var response = await client.PostAsync(IssueAccessTokenUrl, new FormUrlEncodedContent(body));
        return await response.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
    }
}