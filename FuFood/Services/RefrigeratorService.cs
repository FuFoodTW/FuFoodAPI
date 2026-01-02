using FuFood.Models.Entities;
using FuFood.Models.Enums;
using FuFood.Repositories;

namespace FuFood.Services;

public class RefrigeratorService(RefrigeratorRepository repository)
{
    public async Task UpdateNameAsync(User user, Guid refrigeratorId, string newName)
    {
        if (newName.Length < 1 || newName.Length > 10)
        {
            throw new ArgumentException("群組名稱長度必須介於 1~10 個字");
        }

        var refrigerator = await repository.GetUserRefrigeratorById(user, refrigeratorId);
        if (refrigerator == null)
        {
            throw new UnauthorizedAccessException("只有擁有者可以修改冰箱名稱");
        }

        // Check 24-hour limit
        var nextAvailableTime = refrigerator.NameUpdatedAt.AddSeconds(30);
        if (DateTime.UtcNow < nextAvailableTime)
        {
            throw new InvalidOperationException("修改群組名稱每 30 秒限一次");
        }

        refrigerator.Name = newName;
        refrigerator.NameUpdatedAt = DateTime.UtcNow;
        refrigerator.UpdatedAt = DateTime.UtcNow;

        await repository.Update(user, refrigerator, newName);
    }

    public async Task DeleteAsync(User owner, Guid refrigeratorId)
    {
        var refrigerator = await repository.GetOwnedRefrigeratorById(owner, refrigeratorId);
        if (refrigerator == null)
        {
            throw new UnauthorizedAccessException("只有擁有者可以刪除冰箱");
        }

        if (refrigerator.IsDefault)
        {
            throw new InvalidOperationException("預設冰箱不可刪除");
        }

        await repository.Delete(owner, refrigeratorId);
    }
}