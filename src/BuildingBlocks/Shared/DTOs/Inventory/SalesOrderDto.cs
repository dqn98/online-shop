namespace Shared.DTOs.Inventory;

public class SalesOrderDto
{
    public required string OrderNo { get; set; }
    
    public required List<SaleItemDto> SaleItems { get; set; }
}