using FuFood.Models.Requests;
using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class ShoppingListItemController(
    ShoppingListRepository shoppingListRepository,
    RefrigeratorRepository refrigeratorRepository,
    ShoppingListItemRepository itemRepository) : Controller
{
    // 列表
    [HttpGet("/api/v1/shopping_lists/{shoppingListId:guid}/items")]
    public async Task<IActionResult> Index(Guid shoppingListId)
    {
        var user = await HttpContext.GetCurrentUser();

        // 1.先拿 shoppingList
        var shoppingList = await shoppingListRepository.GetById(shoppingListId);
        if (shoppingList == null) return NotFound();

        //2. 從 shoppingList 拿 refrigeratorId
        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, shoppingList.RefrigeratorId);
        if (refrigerator == null) return NotFound();

        // 3. 拿 items
        var items = await itemRepository.GetItemByShoppingListId(shoppingListId);

        return Ok(new
        {
            Data = items
        });
    }

    // 建立共享清單內容 
    [HttpPost("/api/v1/shopping_lists/{shoppingListId:guid}/items")]
    public async Task<IActionResult> Create(Guid shoppingListId, [FromBody] CreateShoppingListItemRequest itemRequest)
    {
        var user = await HttpContext.GetCurrentUser();

        var list = await shoppingListRepository.GetById(shoppingListId);
        if (list == null) return NotFound();

        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, list.RefrigeratorId);
        if (refrigerator == null) return NotFound();

        var item = await itemRepository.CreateListItem(list, user, itemRequest);

        return Ok(new
        {
            Data = item
        });
    }

    [HttpPut("/api/v1/shopping_list_items/{itemId:Guid}/")]
    public async Task<IActionResult> Update(
        Guid itemId,
        [FromBody] UpsertShoppingListItemRequest request)
    {
        var user = await HttpContext.GetCurrentUser();

        var item = await itemRepository.GetItemById(itemId);
        if (item?.ShoppingList == null)
        {
            return NotFound();
        }

        var refrigerator = await refrigeratorRepository
            .GetUserRefrigeratorById(user, item.ShoppingList.RefrigeratorId);

        if (refrigerator == null) return NotFound();

        var listItem = await itemRepository.Update(item, request);

        return Ok(new
        {
            Data = listItem
        });
    }

    [HttpDelete("/api/v1/shopping_list_items/{itemId:Guid}/")]
    public async Task<IActionResult> Delete(Guid itemId)
    {
        var user = await HttpContext.GetCurrentUser();

        var item = await itemRepository.GetItemById(itemId);
        if (item == null) return NotFound();

        var refrigerator = await refrigeratorRepository
            .GetUserRefrigeratorById(user, item.ShoppingList!.RefrigeratorId);

        if (refrigerator == null) return NotFound();

        await itemRepository.Delete(item);

        return Ok();
    }
}