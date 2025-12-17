using FuFood.Controllers;
using FuFood.Models.Requests;
using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;
using FuFood.Models.Entities;

public class RefrigeratorController(RefrigeratorRepository repository) : Controller
{
    // 列出所有自己建立的冰箱群組
    [HttpGet("/api/v1/refrigerators")]
    public async Task<IActionResult> Index()
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerators = await repository.ListUserRefrigerators(user!);
        return Ok(new
        {
            Data = refrigerators
        });
    }

    // 顯示點選的單一群組
    [HttpGet("/api/v1/refrigerators/{id}")]
    public async Task<IActionResult> Show(Guid id)
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerator = await repository.GetUserRefrigeratorById(user!, id);
        if (refrigerator == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            Data = refrigerator
        });
    }

    [HttpPost("/api/v1/refrigerators")]
    public async Task<IActionResult> Create([FromBody] RefrigeratorCreateRequest request)
    {
        var user = await HttpContext.GetCurrentUser();

        var refrigerator = new Refrigerator
        {
            Name = request.Name,
            Colour = request.Colour,
            CreatedById = user!.Id
        };

        var result = await repository.Create(user!, refrigerator);

        return CreatedAtAction(nameof(Show), new { id = result.Id }, new
        {
            Data = result
        });
    }

    [HttpPut("/api/v1/refrigerators/{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RefrigeratorUpdateRequest request)
    {
        var user = await HttpContext.GetCurrentUser();

        var success = await repository.Update(user!, id, request.Name, request.Colour);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("/api/v1/refrigerators/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await HttpContext.GetCurrentUser();

        var success = await repository.Delete(user!, id);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}