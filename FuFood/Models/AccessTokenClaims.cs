using System.Text.Json.Serialization;

namespace FuFood.Models;

public class AccessTokenClaims
{
    [JsonPropertyName("sub")] public Guid Subject { get; set; }
    [JsonPropertyName("iss")] public long IssuedAt { get; set; }
    [JsonPropertyName("exp")] public long ExpiresAt { get; set; }
}