using Shared.Enums.Inventory;

namespace Shared.DTOs.Inventory;

public class SaleItemDto
{
    public string ItemNo { get; set; }
    public decimal Quantity { get; set; }
    public EDocumentType DocumentType { get; set; }
}