using System.ComponentModel.DataAnnotations;
using System.Net;
using Inventory.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Inventory;
using Shared.SeedWork;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    
    public InventoryController(IInventoryService inventoryService)
    {
        ArgumentNullException.ThrowIfNull(inventoryService);
        
        _inventoryService = inventoryService;
    }
    
    [Route("items/{itemNo}", Name = "GetAllByItemNo")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InventoryEntryDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<InventoryEntryDto>>> GetAllByItemNo([Required] string itemNo, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetAllByItemNoAsync(itemNo, cancellationToken);
        return Ok(result);
    }    
    
    [Route("items/{itemNo}/paging", Name = "GetAllByItemNoPaging")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InventoryEntryDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<PagedList<InventoryEntryDto>>> GetAllByItemNoPaging(
        [Required] string itemNo, 
        [FromQuery] GetInventoryPagingQuery query,
        CancellationToken cancellationToken)
    {
        query.SetItemNo(itemNo);
        var result = await _inventoryService.GetAllByItemNoPagingAsync(query, cancellationToken);
        return Ok(result);
    }   
    
    [Route("{id}", Name = "GetInventoryById")]
    [HttpGet]
    [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> GetById([Required] string id, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }
    
    [Route("purchase/{itemNo}", Name = "PurchaseOrder")]
    [HttpPost]
    [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> PurchaseItem(
        [Required] string itemNo, 
        [FromBody] PurchaseProductDto model,
        CancellationToken cancellationToken)
    {
        var result = await _inventoryService.PurchaseItemAsync(itemNo, model, cancellationToken);
        return Ok(result);
    }
    
    [Route("sales/{itemNo}", Name = "SalesItem")]
    [HttpPost]
    [ProducesResponseType(typeof(InventoryEntryDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> SalesItem(
        [Required] string itemNo, 
        [FromBody] SalesProductDto model,
        CancellationToken cancellationToken)
    {
        model.SetItemNo(itemNo);
        var result = await _inventoryService.SalesItemAsync(itemNo, model, cancellationToken);
        return Ok(result);
    }
    
        
    [Route("sales/order-no/{orderNo}", Name = "SalesOrder")]
    [HttpPost]
    [ProducesResponseType(typeof(CreatedSalesOrderSuccessDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<InventoryEntryDto>> SalesOrder(
        [Required] string orderNo, 
        [FromBody] SalesOrderDto model)
    {
        model.OrderNo = orderNo;
        var documentNo = await _inventoryService.SaleOrderAsync(model);
        var result = new CreatedSalesOrderSuccessDto(documentNo);
        return Ok(result);
    }
    
    [Route("{id}", Name = "DeleteInventoryById")]
    [HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeleteById([Required] string id, CancellationToken cancellationToken)
    {
        var entity = await _inventoryService.GetByIdAsync(id, cancellationToken);
        
        if (entity == null) return NotFound();
        
        await _inventoryService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}