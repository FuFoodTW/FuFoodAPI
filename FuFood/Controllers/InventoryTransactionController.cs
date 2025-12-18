using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class InventoryTransactionController(
    RefrigeratorRepository refrigeratorRepository,
    InventoryTransactionRepository inventoryTransactionRepository) : Controller
{
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
}