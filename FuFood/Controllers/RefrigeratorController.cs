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
[Route("api/v1/refrigerators")]
public class RefrigeratorController(RefrigeratorRepository repository, RefrigeratorService service) : ControllerBase
{
    // 列出所有自己加入的冰箱群組 (包含自己擁有的)
    [HttpGet]
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
    [HttpGet("{id:guid}")]
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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RefrigeratorCreateRequest request)
    {
        var user = await HttpContext.GetCurrentUser();

        var refrigerator = new Refrigerator
        {
            Name = request.Name,
            Colour = request.Colour,
            CreatedById = user.Id,
            QrCode = Guid.NewGuid().ToString("N")[..8].ToUpper()
        };

        // 透過導覽屬性自動關聯
        refrigerator.Members.Add(new RefrigeratorMember
        {
            MemberId = user.Id,
            JoinedAt = DateTime.UtcNow
        });

        var result = await repository.Create(user, refrigerator);

        return CreatedAtAction(nameof(Show), new { id = result.Id }, new
        {
            Data = result
        });
    }

    [HttpGet("{id:guid}/qrcode")]
    public async Task<IActionResult> GetQrCode(Guid id)
    {
        var user = await HttpContext.GetCurrentUser();
        var refrigerator = await repository.GetByIdAsync(id);
        
        if (refrigerator == null || !await repository.IsMemberAsync(id, user.Id))
        {
            return NotFound();
        }

        return Ok(new { Data = refrigerator.QrCode });
    }

    [HttpPost("join")]
    public async Task<IActionResult> Join([FromBody] RefrigeratorJoinRequest request)
    {
        var user = await HttpContext.GetCurrentUser();
        try
        {
            await service.JoinByQrCodeAsync(user, request.QrCode);
            return Ok(new { Message = "成功加入冰箱" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/leave")]
    public async Task<IActionResult> Leave(Guid id, [FromBody] RefrigeratorLeaveRequest request)
    {
        var user = await HttpContext.GetCurrentUser();
        try
        {
            await service.LeaveAsync(user, id, request.NewOwnerId);
            return Ok(new { Message = "成功退出冰箱" });
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}/members/{memberId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid id, Guid memberId)
    {
        var user = await HttpContext.GetCurrentUser();
        try
        {
            await service.RemoveMemberAsync(user, id, memberId);
            return Ok(new { Message = "成功移除成員" });
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or InvalidOperationException or KeyNotFoundException)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RefrigeratorUpdateRequest request)
    {
        var user = await HttpContext.GetCurrentUser();
        try
        {
            await service.UpdateNameAsync(user, id, request.Name);
            var refrigerator = await repository.GetByIdAsync(id);
            return Ok(new { Data = refrigerator });
        }
        catch (Exception ex) when (ex is ArgumentException or UnauthorizedAccessException or InvalidOperationException)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
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
