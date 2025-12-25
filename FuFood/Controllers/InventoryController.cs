using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class InventoryController : Controller
{
    [HttpGet("/api/v1/refrigerators/{refrigeratorId:guid}/inventory")]
    public async Task<IActionResult> Index(Guid refrigeratorId)
    {
        var result = new List<string>();
        result.Append("test");
        return Ok(new
        {
            Data = result
        });
    }
}