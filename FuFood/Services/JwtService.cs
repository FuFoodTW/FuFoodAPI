using FuFood.Models;
using JWT.Algorithms;
using JWT.Builder;

namespace FuFood.Services;

public class JwtService(CryptoService cryptoService)
{
    public string IssueAccessTokenForUser(User user)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(cryptoService.AccessTokenSigner)
            .AddClaim("sub", user.Id.ToString())
            .AddClaim("iat", now)
            .AddClaim("exp", now + 3600)
            .Encode();
    }
}