using FuFood.Models;
using JWT.Algorithms;
using JWT.Builder;

namespace FuFood.Services;

public class JwtService(CryptoService cryptoService)
{
    // 使用者登入時產生的 token
    public string IssueAccessTokenForUser(User user)
    {
        var now = DateTime.UtcNow;
        return JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(cryptoService.AccessTokenSigner)
            .Subject(user.Id.ToString())
            .IssuedAt(now)
            .ExpirationTime(now + TimeSpan.FromDays(1))
            .Encode();
    }

    // 驗證請求的 token 是否為真
    public AccessTokenClaims DecodeAccessToken(string token)
    {
        return JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(cryptoService.AccessTokenSigner)
            .Decode<AccessTokenClaims>(token);
    }
}