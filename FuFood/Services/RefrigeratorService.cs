using FuFood.Models.Entities;
using FuFood.Models.Enums;
using FuFood.Repositories;

namespace FuFood.Services;

public class RefrigeratorService(RefrigeratorRepository repository, UserRepository userRepository)
{
    private const int FreeSubscriptionLimit = 3;
    private const int ProSubscriptionLimit = 5;

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
        var nextAvailableTime = refrigerator.NameUpdatedAt.AddHours(24);
        if (DateTime.UtcNow < nextAvailableTime)
        {
            throw new InvalidOperationException("修改群組名稱每 24 小時限一次");
        }

        refrigerator.Name = newName;
        refrigerator.NameUpdatedAt = DateTime.UtcNow;
        refrigerator.UpdatedAt = DateTime.UtcNow;

        await repository.Update(user, refrigerator, newName, refrigerator.Colour);
    }

    public async Task LeaveAsync(User user, Guid refrigeratorId, Guid? newOwnerId = null)
    {
        var refrigerator = await repository.GetByIdAsync(refrigeratorId);
        if (refrigerator == null) throw new KeyNotFoundException("冰箱不存在");

        if (refrigerator.OwnerId == user.Id)
        {
            // Owner leaving
            if (refrigerator.IsDefault)
            {
                throw new InvalidOperationException("預設冰箱的擁有者不可移除自己");
            }

            if (newOwnerId == null)
            {
                throw new ArgumentException("非預設冰箱的擁有者移除自己時必須指定新擁有者");
            }

            // Verify new owner is a member
            if (!await repository.IsMemberAsync(refrigeratorId, newOwnerId.Value))
            {
                throw new InvalidOperationException("指定的對象必須是該冰箱的成員");
            }

            await repository.UpdateOwnershipAsync(refrigeratorId, newOwnerId.Value);
        }

        await repository.RemoveMemberAsync(refrigeratorId, user.Id);

        // TODO: Notification logic
    }

    public async Task RemoveMemberAsync(User owner, Guid refrigeratorId, Guid memberId)
    {
        var refrigerator = await repository.GetUserRefrigeratorById(owner, refrigeratorId);
        if (refrigerator == null)
        {
            throw new UnauthorizedAccessException("只有擁有者可以移除成員");
        }

        if (owner.Id == memberId)
        {
            throw new InvalidOperationException("擁有者不能透過此管道移除自己，請使用退出功能");
        }

        var success = await repository.RemoveMemberAsync(refrigeratorId, memberId);
        if (!success)
        {
            throw new KeyNotFoundException("該會員不是此冰箱的成員");
        }

        // TODO: Notification logic
    }

    public async Task DeleteAsync(User owner, Guid refrigeratorId)
    {
        var refrigerator = await repository.GetUserRefrigeratorById(owner, refrigeratorId);
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