using Microsoft.AspNetCore.Mvc;

namespace Saga.Orchestrator.Controllers;

public class CheckoutController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}