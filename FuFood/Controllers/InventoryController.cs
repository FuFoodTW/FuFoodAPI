using FuFood.Models.Enums;
using FuFood.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

// 看冰箱內容的 controller
public class InventoryController(
    InventoryQueryRepository inventoryQueryRepository,
    RefrigeratorRepository refrigeratorRepository) : Controller
{
    // 看庫存列表
    [HttpGet("/api/v1/refrigerators/{refrigeratorId:guid}/inventory")]
    public async Task<IActionResult> GetInventory(Guid refrigeratorId, [FromQuery] ProductCategory category)
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, refrigeratorId);
        if (refrigerator == null)
        {
            return NotFound();
        }

        var result = await inventoryQueryRepository.GetInventoryByCategory(refrigeratorId, category);

        return Ok(new
        {
            Data = result
        });
    }

    // 看品項詳細
    [HttpGet("api/v1/refrigerators/{refrigeratorId:guid}/products/{productId:guid}")]
    public async Task<IActionResult> GetProductDetail(Guid refrigeratorId, Guid productId)
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, refrigeratorId);
        if (refrigerator == null)
        {
            return NotFound();
        }

        var result = await inventoryQueryRepository.GetProductInventoryDetail(refrigeratorId, productId);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            Data = result
        });
    }
}