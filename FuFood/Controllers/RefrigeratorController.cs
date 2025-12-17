using FuFood.Controllers;
using FuFood.Repositories;
using Microsoft.AspNetCore.Mvc;

public class RefrigeratorController(RefrigeratorRepository repository) : Controller
{
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
}