using System.Buffers.Text;
using System.Security.Cryptography;
using FuFood.Data;
using FuFood.Models.Entities;

namespace FuFood.Services;

public class RefrigeratorInvitationService(AppDbContext dbContext, CryptoService cryptoService)
{
    public async Task<RefrigeratorInvitation> CreateRefrigeratorInvitation(User user, Refrigerator refrigerator)
    {
        var token = GenerateInvitationToken();
        var tokenHash = cryptoService.HashInvitationToken(token);

        var invitation = new RefrigeratorInvitation
        {
            CreatorId = user.Id,
            RefrigeratorId = refrigerator.Id,
            Token = token,
            TokenHash = tokenHash,
        };

        dbContext.Add(invitation);
        await dbContext.SaveChangesAsync();
        return invitation;
    }

    private static string GenerateInvitationToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(24);
        return Base64Url.EncodeToString(bytes);
    }
}