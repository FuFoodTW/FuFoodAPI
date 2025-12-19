using FuFood.Models.Entities;
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
        if (transaction == null) return NotFound("Transaction not found");

        var item = new InventoryTransactionItem
        {
            InventoryTransaction = transaction,
            Quantity = createRequest.Quantity,
            ExpirationDate = createRequest.ExpirationDate,
            InventoryTransactionItemImage = createRequest.Image
        };

        if (createRequest.ProductId != null)
        {
            var product = await productRepository.GetById(createRequest.ProductId.Value);
            if (product == null)
            {
                return UnprocessableEntity("Invalid product ID");
            }

            item.Product = product;
        }

        if (createRequest.ProductParams != null)
        {
            item.Product = new Product
            {
                Name = createRequest.ProductParams.Name,
                Quantity = createRequest.ProductParams.Quantity,
                Unit = createRequest.ProductParams.Unit,
            };
        }

        item = await inventoryTransactionItemRepository.Create(item);
        return Ok(new
        {
            Data = item
        });
    }
}