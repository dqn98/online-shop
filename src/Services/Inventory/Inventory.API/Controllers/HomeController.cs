using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

public class HomeController : ControllerBase
{
    // GET
    public IActionResult Index()
    {
        return new RedirectResult("~/swagger");
    }
}