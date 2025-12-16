using System.Text.Json.Serialization;

namespace FuFood.Models;

public class LineIdTokenClaims
{
    [JsonPropertyName("iss")] public string? Issuer { get; set; }
    [JsonPropertyName("sub")] public required string Subject { get; set; }
    [JsonPropertyName("aud")] public string? Audience { get; set; }
    [JsonPropertyName("exp")] public int ExpiresAt { get; set; }
    [JsonPropertyName("iat")] public int IssuedAt { get; set; }
    [JsonPropertyName("amr")] public string[]? Amr { get; set; }
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("picture")] public string? Picture { get; set; }
}