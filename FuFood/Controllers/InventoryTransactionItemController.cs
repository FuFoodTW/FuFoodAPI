using FuFood.Models.Requests;
using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuFood.Controllers;

public class InventoryTransactionItemController(
    InventoryTransactionRepository inventoryTransactionRepository,
    InventoryTransactionItemRepository inventoryTransactionItemRepository,
    ProductRepository productRepository)
    : Controller
{
    [HttpPost("/api/v1/inventory_transactions/{transactionId:guid}/items")]
    public async Task<IActionResult> Create(Guid transactionId,
        [FromBody] InventoryTransactionItemCreateRequest createRequest)
    {
        // 找當下使用者,名下的冰箱有沒有此筆入庫交易
        var user = await HttpContext.GetCurrentUser();
        var transaction = await inventoryTransactionRepository.GetUserInventoryTransaction(user, transactionId);
        if (transaction == null)
        {
            return NotFound("Transaction not found");
        }

        var product = await productRepository.GetById(createRequest.ProductId);

        if (product == null)
        {
            return NotFound("Product not found");
        }

        var item = await inventoryTransactionItemRepository.Create(transaction, product, createRequest.Quantity,
            createRequest.ExpirationDate, createRequest.Image);
        return Ok(new
        {
            Data = item
        });
    }
}