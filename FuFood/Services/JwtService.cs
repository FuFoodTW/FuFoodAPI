using FuFood.Models;
using JWT.Algorithms;
using JWT.Builder;

namespace FuFood.Services;

public class JwtService(CryptoService cryptoService)
{
    public string IssueAccessTokenForUser(User user)
    {
        return JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(cryptoService.AccessTokenSigner)
            .Subject(user.Id.ToString())
            .IssuedAt(DateTime.Now)
            .ExpirationTime(DateTime.Now + TimeSpan.FromHours(24))
            .Encode();
    }

    public AccessTokenClaims DecodeAccessToken(string accessToken)
    {
        return JwtBuilder.Create()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(cryptoService.AccessTokenSigner)
            .Decode<AccessTokenClaims>(accessToken);
    }
}