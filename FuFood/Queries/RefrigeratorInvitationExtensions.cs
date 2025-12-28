using FuFood.Models.Entities;

namespace FuFood.Queries;

public static class RefrigeratorInvitationExtensions
{
    extension(IQueryable<RefrigeratorInvitation> query)
    {
        public IQueryable<RefrigeratorInvitation> Active()
        {
            return query.Where(i => i.ExpiresAt > DateTime.UtcNow);
        }

        public IQueryable<RefrigeratorInvitation> Expired()
        {
            return query.Where(i => i.ExpiresAt < DateTime.UtcNow);
        }
    }
}