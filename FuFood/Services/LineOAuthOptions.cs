namespace FuFood.Services;

public class LineOAuthOptions
{
    public const string SectionName = "LineOAuth";

    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string CallbackUrl { get; set; }
}