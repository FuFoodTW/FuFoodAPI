using FuFood.Models.Requests;
using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class ShoppingListController(
    RefrigeratorRepository refrigeratorRepository,
    ShoppingListRepository shoppingListRepository) : Controller
{
    // 所有共享清單列表
    [HttpGet("/api/v1/refrigerators/{refrigeratorId:guid}/shopping_lists")]
    public async Task<IActionResult> Index(Guid refrigeratorId)
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, refrigeratorId);
        if (refrigerator == null) return NotFound();

        var lists = await shoppingListRepository.GetListsByRefrigerator(refrigeratorId);

        return Ok(new
        {
            Data = lists
        });
    }

    // 單一共享清單內容
    [HttpGet("/api/v1/shopping_lists/{shoppingListId:guid}")]
    public async Task<IActionResult> Show(Guid shoppingListId)
    {
        var user = await HttpContext.GetCurrentUser();

        var list = await shoppingListRepository.GetById(shoppingListId);
        if (list == null) return NotFound();

        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, list.RefrigeratorId);

        if (refrigerator == null) return NotFound();

        return Ok(new
        {
            Data = list
        });
    }

    // 建立共享清單
    [HttpPost("/api/v1/refrigerators/{refrigeratorId:guid}/shopping_lists")]
    public async Task<IActionResult> Create(Guid refrigeratorId,
        [FromBody] CreateShoppingListRequest shoppingListRequest)
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, refrigeratorId);
        if (refrigerator == null) return NotFound();

        var list = await shoppingListRepository.CreateShoppingList(refrigerator, shoppingListRequest);

        return Ok(new
        {
            Data = list
        });
    }

    // 編輯共享清單
    [HttpPut("/api/v1/shopping_lists/{shoppingListId:guid}")]
    public async Task<IActionResult> Update(Guid shoppingListId,
        [FromBody] UpsertShoppingListRequest shoppingListRequest)
    {
        var user = await HttpContext.GetCurrentUser();

        var list = await shoppingListRepository.GetById(shoppingListId);
        if (list == null) return NotFound();

        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, list.RefrigeratorId);

        if (refrigerator == null) return NotFound();

        var shoppingList = await shoppingListRepository.UpdateShoppingList(list, shoppingListRequest);

        return Ok(new
        {
            Data = shoppingList
        });
    }

    // 刪除共享清單
    [HttpDelete("/api/v1/shopping_lists/{shoppingListId:guid}")]
    public async Task<IActionResult> Delete(Guid shoppingListId)
    {
        var user = await HttpContext.GetCurrentUser();

        var list = await shoppingListRepository.GetById(shoppingListId);
        if (list == null) return NotFound();

        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, list.RefrigeratorId);

        if (refrigerator == null) return NotFound();

        await shoppingListRepository.Delete(list);

        return Ok();
    }
}