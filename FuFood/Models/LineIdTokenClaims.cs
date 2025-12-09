using System.Text.Json.Serialization;

namespace FuFood.Models;

public class LineIdTokenClaims
{
    [JsonPropertyName("iss")] public required string Issuer { get; set; }
    [JsonPropertyName("sub")] public required string Subject { get; set; }
    [JsonPropertyName("aud")] public required string Audience { get; set; }
    [JsonPropertyName("exp")] public int ExpiresAt { get; set; }
    [JsonPropertyName("iat")] public int IssuedAt { get; set; }
    [JsonPropertyName("amr")] public required string[] Amr { get; set; }
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("picture")] public required string Picture { get; set; }
}