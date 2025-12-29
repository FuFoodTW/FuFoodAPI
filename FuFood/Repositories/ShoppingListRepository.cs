using FuFood.Data;
using FuFood.Models.Entities;
using FuFood.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class ShoppingListRepository(
    AppDbContext dbContext)
{
    public async Task<ShoppingList?> GetById(Guid id)
    {
        return await dbContext.ShoppingLists
            .Include(x => x.Items)
            .Include(x => x.Refrigerator)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<ShoppingList>> GetListsByRefrigerator(Guid refrigeratorId)
    {
        return await dbContext.ShoppingLists
            .Where(r => r.RefrigeratorId == refrigeratorId)
            .OrderBy(r => r.Id)
            .ToListAsync();
    }

    public async Task<ShoppingList> CreateShoppingList(
        Refrigerator refrigerator, CreateShoppingListRequest shoppingListRequest)
    {
        var list = new ShoppingList
        {
            RefrigeratorId = refrigerator.Id,
            Title = shoppingListRequest.Title,
            EnableNotifications = shoppingListRequest.EnableNotifications,
            CoverPhotoPath = shoppingListRequest.CoverPhotoPath,
            StartsAt = shoppingListRequest.StartsAt
        };
        dbContext.ShoppingLists.Add(list);
        await dbContext.SaveChangesAsync();
        return list;
    }

    public async Task<ShoppingList> UpdateShoppingList(ShoppingList shoppingList,
        UpsertShoppingListRequest upsertShoppingListRequest)
    {
        shoppingList.Title = upsertShoppingListRequest.Title;
        shoppingList.CoverPhotoPath = upsertShoppingListRequest.CoverPhotoPath;
        shoppingList.StartsAt = upsertShoppingListRequest.StartsAt;
        shoppingList.EnableNotifications = upsertShoppingListRequest.EnableNotifications;
        shoppingList.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return shoppingList;
    }

    public async Task Delete(ShoppingList list)
    {
        dbContext.ShoppingLists.Remove(list);
        await dbContext.SaveChangesAsync();
    }
}