using FuFood.Data;
using FuFood.Models.Entities;
using FuFood.Models.Requests;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Repositories;

public class ShoppingListItemRepository(AppDbContext dbContext)
{
    public async Task<ShoppingListItem?> GetById(Guid id)
    {
        return await dbContext.ShoppingListItems.Include(x => x.ShoppingList).ThenInclude(l => l!.Refrigerator)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ShoppingListItem> CreateListItem(ShoppingList shoppingList, User user,
        CreateShoppingListItemRequest itemRequest)
    {
        var item = new ShoppingListItem
        {
            ShoppingListId = shoppingList.Id,
            CreatorId = user.Id,
            Name = itemRequest.Name,
            Quantity = itemRequest.Quantity,
            Unit = itemRequest.Unit,
            PhotoPath = itemRequest.PhotoPath
        };

        dbContext.ShoppingListItems.Add(item);
        await dbContext.SaveChangesAsync();

        return item;
    }

    public async Task<ShoppingListItem> Update(
        ShoppingListItem item,
        UpsertShoppingListItemRequest request)
    {
        item.Name = request.Name;
        item.Quantity = request.Quantity;
        item.Unit = request.Unit;
        item.UpdatedAt = DateTime.UtcNow;
        item.PhotoPath = request.PhotoPath;

        await dbContext.SaveChangesAsync();
        return item;
    }

    public async Task Delete(ShoppingListItem item)
    {
        dbContext.ShoppingListItems.Remove(item);
        await dbContext.SaveChangesAsync();
    }
}