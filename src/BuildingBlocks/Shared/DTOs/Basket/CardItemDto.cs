using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Basket;

public class CardItemDto
{
    [Range(1, double.PositiveInfinity, ErrorMessage = "The field {0} must be >= {1}.")]
    public required int Quantity { get; set; }
    
    [Range(0.1, double.PositiveInfinity, ErrorMessage = "The field {0} must be >= {1}.")]
    public required decimal ItemPrice { get; set; }
    
    public required string ItemNo { get; set; }
    
    public required string ItemName { get; set; }

    public int AvailableQuantity { get; set; }

    public void SetAvailableQuantity(int quantity)
    {
        AvailableQuantity = quantity;
    }
}