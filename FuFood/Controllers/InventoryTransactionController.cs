using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class InventoryTransactionController(
    RefrigeratorRepository refrigeratorRepository,
    InventoryTransactionRepository inventoryTransactionRepository) : Controller
{
    // 建立一筆交易
    [HttpPost("/api/v1/refrigerators/{refrigeratorId:guid}/inventory_transactions")]
    public async Task<IActionResult> Create(Guid refrigeratorId)
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, refrigeratorId);
        if (refrigerator == null)
        {
            return NotFound();
        }

        var transaction = await inventoryTransactionRepository.CreateUserInventoryTransaction(user, refrigerator);
        return Ok(new
        {
            Data = transaction
        });
    }

    // 簡易列表
    [HttpGet("/api/v1/refrigerators/{refrigeratorId:guid}/inventory_transactions")]
    public async Task<IActionResult> List(Guid refrigeratorId)
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, refrigeratorId);
        if (refrigerator == null)
        {
            return NotFound();
        }

        var transactions = await inventoryTransactionRepository.GetByRefrigerator(refrigeratorId);
        return Ok(new
        {
            Data = transactions
        });
    }

    // 詳細列表
    [HttpGet("/api/v1/inventory_transactions/{transactionId:guid}")]
    public async Task<IActionResult> Detail(Guid transactionId)
    {
        var user = await HttpContext.GetCurrentUser();
        var transaction = await inventoryTransactionRepository.GetUserInventoryTransaction(user, transactionId);
        if (transaction == null)
        {
            return NotFound();
        }

        var detail = await inventoryTransactionRepository.GetByRefrigerator(transactionId);
        return Ok(new
        {
            Data = detail
        });
    }
}