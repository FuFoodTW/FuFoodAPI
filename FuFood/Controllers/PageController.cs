using Microsoft.AspNetCore.Mvc;

namespace FuFood.Controllers;

public class PageController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }
}