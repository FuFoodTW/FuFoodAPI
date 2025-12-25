using FuFood.Models.Enums;
using FuFood.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

[ApiController]
[Route("api/v1/refrigerators/{refrigeratorId:guid}/products")]
public class ProductsController(ProductService service) : ControllerBase
{
    // GET /api/refrigerators/{id}/products?categories=Vegetable,Frozen&page=1&pageSize=20
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        Guid refrigeratorId,
        [FromQuery] ProductCategory? categories,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool orderDescending = true)
    {
        var products = await service.GetProductsAsync(
            refrigeratorId, categories, page, pageSize, orderDescending);

        return Ok(new
        {
            Data = products
        });
    }

    // GET /api/refrigerators/{id}/products/categories
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(Guid refrigeratorId)
    {
        var categories = await service.GetCategoriesAsync(refrigeratorId);
        return Ok(new
        {
            Data = categories
        });
    }
}