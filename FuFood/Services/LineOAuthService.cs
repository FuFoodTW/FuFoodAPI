using Microsoft.Extensions.Options;

namespace FuFood.Services;

public class LineOAuthService(HttpClient client, IOptions<LineOAuthOptions> options)
{
    private readonly LineOAuthOptions _options = options.Value;
}