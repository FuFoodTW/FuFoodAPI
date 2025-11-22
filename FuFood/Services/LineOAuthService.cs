using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace FuFood.Services;

public class LineOAuthService(HttpClient client, IOptions<LineOAuthOptions> options)
{
    private readonly LineOAuthOptions _options = options.Value;
    public const string AuthorizeBaseUrl = "https://access.line.me/oauth2/v2.1/authorize";

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
}