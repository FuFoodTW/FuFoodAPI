using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;
using FuFood.Models.Requests;

namespace FuFood.Controllers;

public class InventoryTransactionController(
    RefrigeratorRepository refrigeratorRepository,
    InventoryTransactionRepository inventoryTransactionRepository,
    InventoryTransactionItemRepository itemRepository) : Controller
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

    [HttpPost("/api/v1/inventory_transactions/{transactionId:guid}/finalize")]
    public async Task<IActionResult> Finalize(Guid transactionId)
    {
        var user = await HttpContext.GetCurrentUser();
        var transaction = await inventoryTransactionRepository.GetDraftTransaction(user, transactionId);
        if (transaction == null) return NotFound();

        if (!await inventoryTransactionRepository.HasItems(transaction))
        {
            return UnprocessableEntity("Only transactions with items can be finalized");
        }

        transaction = await inventoryTransactionRepository.FinalizeTransaction(transaction);
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
            .GetConsumableTransaction(user, transactionId);

        if (transaction == null)
            return NotFound("The requested transaction does not exist or is not in a consumable state");

        try
        {
            var result = await itemRepository.Consume(
                transaction,
                request.InventoryItemId,
                request.Quantity
            );

            return Ok(new { Data = result });
        }
        catch (Exception ex)
        {
            return UnprocessableEntity(ex.Message);
        }
    }
}