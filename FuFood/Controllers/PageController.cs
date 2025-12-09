using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class PageController : Controller
{
    [HttpGet("/")]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
}