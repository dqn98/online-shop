using Shared.Enums.Inventory;

namespace Shared.DTOs.Inventory;

public sealed class PurchaseProductDto
{
    public string? ItemNo { get; set; }
    public string? DocumentNo { get; set; }
    public string? ExternalDocumentNo { get; set; }
    public EDocumentType DocumentType { get; set; } = EDocumentType.Purchase;
    public double Quantity { get; set; }
}