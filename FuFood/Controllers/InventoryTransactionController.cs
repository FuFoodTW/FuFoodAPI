using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;
using FuFood.Models.Requests;
using FuFood.Services;

namespace FuFood.Controllers;

public class InventoryTransactionController(
    RefrigeratorRepository refrigeratorRepository,
    InventoryTransactionRepository inventoryTransactionRepository,
    InventoryTransactionItemRepository itemRepository,
    CommitTransactionService commitTransactionService) : Controller
{
    // 建立一筆交易
    [HttpPost("/api/v1/refrigerators/{refrigeratorId:guid}/inventory_transactions")]
    public async Task<IActionResult> Create(Guid refrigeratorId)
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, refrigeratorId);
        if (refrigerator == null) return NotFound();

        var transaction = await inventoryTransactionRepository.CreateUserInventoryTransaction(user, refrigerator);
        return Ok(new
        {
            Data = transaction
        });
    }

    [HttpPost("/api/v1/inventory_transactions/{transactionId:guid}/commit")]
    public async Task<IActionResult> FinalizeTransaction(Guid transactionId)
    {
        var user = await HttpContext.GetCurrentUser();

        var transaction = await inventoryTransactionRepository.GetPendingTransaction(user, transactionId);
        if (transaction == null)
        {
            return NotFound("No pending transaction found with the given ID.");
        }

        try
        {
            transaction = await commitTransactionService.CommitTransaction(user, transaction);
            return Ok(new
            {
                Data = transaction
            });
        }
        catch (InvalidOperationException e)
        {
            return UnprocessableEntity(e.Message);
        }
    }

    // 簡易列表
    [HttpGet("/api/v1/refrigerators/{refrigeratorId:guid}/inventory_transactions")]
    public async Task<IActionResult> List(Guid refrigeratorId)
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerator = await refrigeratorRepository.GetUserRefrigeratorById(user, refrigeratorId);
        if (refrigerator == null) return NotFound();

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
        if (transaction == null) return NotFound();

        var detail = await inventoryTransactionRepository.GetByRefrigerator(transactionId);
        return Ok(new
        {
            Data = detail
        });
    }

// 消耗庫存
    [HttpPost("/api/v1/inventory_transactions/{transactionId:guid}/consume")]
    public async Task<IActionResult> Consume(
        Guid transactionId,
        [FromBody] InventoryConsumeRequest request)
    {
        var user = await HttpContext.GetCurrentUser();

        var transaction = await inventoryTransactionRepository
            .GetPendingTransaction(user, transactionId);

        if (transaction == null)
            return NotFound("No pending transaction found with the requested ID.");

        try
        {
            var result = await itemRepository.Consume(
                transaction,
                request.InventoryItemId,
                request.Quantity
            );

            return Ok(new { Data = result });
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
    }

    [HttpDelete("/api/v1/inventory_transactions/{transactionId:guid}")]
    public async Task<IActionResult> Delete(Guid transactionId)
    {
        var user = await HttpContext.GetCurrentUser();

        var transaction = await inventoryTransactionRepository.GetPendingTransaction(user, transactionId);

        if (transaction == null)
        {
            return NotFound("No pending transaction found with the requested ID.");
        }

        await inventoryTransactionRepository.DeleteTransaction(transaction);

        return NoContent();
    }
}