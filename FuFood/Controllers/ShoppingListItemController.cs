using FuFood.Models.Requests;
using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class ShoppingListItemController(
    ShoppingListRepository shoppingListRepository,
    RefrigeratorRepository refrigeratorRepository,
    ShoppingListItemRepository itemRepository) : Controller
{
    [HttpGet("/api/v1/shopping_lists/{shoppingListId:guid}/items")]
    public async Task<IActionResult> Create(Guid shoppingListId, [FromBody] UpsertShoppingListItemRequest itemRequest)
    {
        var user = await HttpContext.GetCurrentUser();

        var list = await shoppingListRepository.GetById(shoppingListId);
        if (list == null) return NotFound();

        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, list.RefrigeratorId);
        if (refrigerator == null) return Forbid();

        var item = await itemRepository.CreateListItem(list, user, itemRequest);

        return Ok(new
        {
            Data = item
        });
    }

    [HttpPut("/api/v1/shopping_list_items/{itemId:Guid}/")]
    public async Task<IActionResult> UpdateItem(
        Guid itemId,
        [FromBody] UpsertShoppingListItemRequest request)
    {
        var user = await HttpContext.GetCurrentUser();

        var item = await itemRepository.GetById(itemId);
        if (item == null) return NotFound();

        var refrigerator = await refrigeratorRepository
            .GetUserRefrigeratorById(user, item.ShoppingList!.RefrigeratorId);

        if (refrigerator == null) return Forbid();

        await itemRepository.Update(item, request);

        return Ok();
    }

    [HttpDelete("/api/v1/shopping_list_items/{itemId:Guid}/")]
    public async Task<IActionResult> DeleteItem(Guid itemId)
    {
        var user = await HttpContext.GetCurrentUser();

        var item = await itemRepository.GetById(itemId);
        if (item == null) return NotFound();

        var refrigerator = await refrigeratorRepository
            .GetUserRefrigeratorById(user, item.ShoppingList!.RefrigeratorId);

        if (refrigerator == null) return Forbid();

        await itemRepository.Delete(item);

        return Ok();
    }
}