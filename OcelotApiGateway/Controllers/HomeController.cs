using Microsoft.AspNetCore.Mvc;

namespace OcelotApiGateway.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Redirect("~/scalar/v1");
    }
}