using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class ShoppingListController : Controller
{
    // 所有共享清單列表
    [HttpGet("/api/v1/refrigerators/{refrigeratorId:guid}/shopping_lists")]
    public async Task<IActionResult> Index(Guid id)
    {
        throw new NotImplementedException();
    }

    // 單一共享清單內容
    [HttpGet("/api/v1/shopping_lists/{shoppingListId:guid}")]
    public async Task<IActionResult> Show(Guid id)
    {
        throw new NotImplementedException();
    }
    

    // 建立共享清單
    // 編輯共享清單
    // 刪除共享清單
}