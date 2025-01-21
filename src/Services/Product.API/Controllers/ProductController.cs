using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.API.Entities;
using Product.API.Repositories.Interfaces;
using Shared.DTOs.Product;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductController(IProductRepository productRepository, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(productRepository, nameof(productRepository));
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));
        
        _productRepository = productRepository;
        _mapper = mapper;
    }

    #region CRUD
    // GET
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productRepository.GetProducts();
        var result = _mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(result);
    }
    
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetProduct([Required]long id)
    {
        var product = await _productRepository.GetProduct(id);

        if(product == null) {
            return NotFound();
        }
        var result = _mapper.Map<ProductDto>(product);

        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
    {
        if(productDto.No == null) return BadRequest("Product No is required");
        
        var productEntity = await _productRepository.GetProductByNo(productDto.No);
        if (productEntity is not null) 
            return BadRequest($"Product No: {productDto.No} is existed.");

        var product = _mapper.Map<CatalogProduct>(productDto);
        await _productRepository.CreateProduct(product);
        await _productRepository.SaveChangesAsync();
        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    [Authorize]
    public async Task<IActionResult> UpdateProduct([Required] long id, [FromBody] UpdateProductDto productDto)
    {
        var product = await _productRepository.GetProduct(id);
        var updateProduct = _mapper.Map(productDto, product);
        if (updateProduct is null)
        {
            return NotFound();
        }
        
        await _productRepository.UpdateProduct(updateProduct);
        await _productRepository.SaveChangesAsync();
        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<IActionResult> DeleteProduct([Required] long id)
    {
        var product = await _productRepository.GetProduct(id);
        if (product == null)
            return NotFound();

        await _productRepository.DeleteProduct(id);
        await _productRepository.SaveChangesAsync();
        return NoContent();
    }

    #endregion
}