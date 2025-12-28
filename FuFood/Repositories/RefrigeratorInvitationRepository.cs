using FuFood.Data;
using FuFood.Models.Entities;
using FuFood.Queries;
using FuFood.Services;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class RefrigeratorInvitationRepository(AppDbContext dbContext, CryptoService cryptoService)
{
    public async Task<RefrigeratorInvitation?> GetInvitationByToken(string token)
    {
        var hash = cryptoService.HashInvitationToken(token);
        return await dbContext.RefrigeratorInvitations
            .Active()
            .Include(i => i.Creator)
            .Include(i => i.Refrigerator)
            .FirstOrDefaultAsync(i => i.TokenHash == hash);
    }

    public async Task Vacuum()
    {
        await dbContext.RefrigeratorInvitations
            .Expired()
            .ExecuteDeleteAsync();
    }
}