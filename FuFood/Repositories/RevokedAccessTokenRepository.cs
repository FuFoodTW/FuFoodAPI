using System.Security.Cryptography;
using System.Text;
using FuFood.Data;
using FuFood.Models;
using FuFood.Services;
using JWT.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class RevokedAccessTokenRepository(AppDbContext dbContext, JwtService jwtService)
{
    public async Task<bool> RevokeAccessToken(string token)
    {
        try
        {
            // if the access token is invalid or expired, there
            // is no need to revoke it
            var claims = jwtService.DecodeAccessToken(token);
            var hash = HashToken(token);

            await dbContext.RevokedAccessTokens.Upsert(new RevokedAccessToken
                {
                    TokenHash = hash,
                    ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(claims.ExpiresAt).DateTime,
                })
                .On(t => t.TokenHash)
                .NoUpdate()
                .RunAsync();

            return true;
        }
        catch (Exception ex) when (ex is SignatureVerificationException or TokenExpiredException)
        {
            return false;
        }
    }

    public async Task<bool> IsTokenRevoked(string token)
    {
        var hash = HashToken(token);

        return await dbContext.RevokedAccessTokens
            .Where(t => t.TokenHash == hash)
            .AnyAsync();
    }

    // We don't store the access token itself, only the hash of it.
    // This way, the hash is only 32 bytes vs a potentially very long access token.
    private static byte[] HashToken(string token)
    {
        var trimmed = token.Trim();
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(trimmed));
    }
}