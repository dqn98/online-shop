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
    
    /// <summary>
    /// api/inventory/items/{itemNo}
    /// </summary>
    /// <param name="itemNo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Route("items/{itemNo}", Name = "GetAllByItemNo")]
    [ProducesResponseType(typeof(IEnumerable<InventoryEntryDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<InventoryEntryDto>>> GetAllByItemNo([Required] string itemNo, CancellationToken cancellationToken)
    {
        var result = await _inventoryService.GetAllByItemNoAsync(itemNo, cancellationToken);
        return Ok(result);
    }    
    
    /// <summary>
    /// api/inventory/items/{itemNo}/paging
    /// </summary>
    /// <param name="itemNo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Route("items/{itemNo}/paging", Name = "GetAllByItemNoPaging")]
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
}