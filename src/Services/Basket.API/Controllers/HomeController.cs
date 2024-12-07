using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return Redirect("/baskets");
    }
}