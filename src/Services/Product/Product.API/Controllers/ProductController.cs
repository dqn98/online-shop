using Microsoft.AspNetCore.Mvc;
using Product.API.Entities;
using Product.API.Repositories.Interfaces;

namespace Product.API.Controllers;

public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;

    public ProductController(IProductRepository productRepository)
    {
        ArgumentNullException.ThrowIfNull(productRepository, nameof(productRepository));
        _productRepository = productRepository;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var products = await _productRepository.GetProducts();
        return View(products);
    }

    [HttpGet("api/products")]
    public async Task<IEnumerable<CatalogProduct>> GetProducts()
    {
        var products = await _productRepository.GetProducts();
        return products;
    }
}