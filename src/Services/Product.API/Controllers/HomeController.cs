using Microsoft.AspNetCore.Mvc;

namespace Product.API.Controllers;

public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}