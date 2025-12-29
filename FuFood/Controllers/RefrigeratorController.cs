using FuFood.Models.Entities;
using FuFood.Models.Requests;
using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;
using FuFood.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using FuFood.Models;

namespace FuFood.Controllers;

[ApiController]
public class RefrigeratorController(RefrigeratorRepository repository, RefrigeratorService service) : ControllerBase
{
    // 列出所有自己加入的冰箱群組 (包含自己擁有的)
    [HttpGet("/api/v1/refrigerators")]
    public async Task<IActionResult> Index()
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerators = await repository.ListUserRefrigerators(user);
        return Ok(new
        {
            Data = refrigerators
        });
    }

    // 顯示點選的單一群組
    [HttpGet("/api/v1/refrigerators/{id:guid}")]
    public async Task<IActionResult> Show(Guid id)
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerator = await repository.GetUserRefrigeratorById(user, id);
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
            OwnerId = user.Id,
        };

        var result = await repository.Create(user, refrigerator);

        return CreatedAtAction(nameof(Show), new { id = result.Id }, new
        {
            Data = result
        });
    }

    [HttpPut("/api/v1/refrigerators/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RefrigeratorUpdateRequest request)
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerator = await repository.GetOwnedRefrigeratorById(user, id);
        if (refrigerator == null)
        {
            return NotFound();
        }

        try
        {
            await service.UpdateNameAsync(user, id, request.Name);
            refrigerator = await repository.GetByIdAsync(id);
            return Ok(new { Data = refrigerator });
        }
        catch (Exception ex) when (ex is ArgumentException or UnauthorizedAccessException or InvalidOperationException)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("/api/v1/refrigerators/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await HttpContext.GetCurrentUser();
        try
        {
            await service.DeleteAsync(user, id);
            return NoContent();
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or InvalidOperationException)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}