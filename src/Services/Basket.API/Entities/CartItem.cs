namespace Basket.API.Entities;

public class CartItem
{
    public int Quantity { get; set; }
    public decimal ItemPrice { get; set; }
    public required string ItemNo { get; set; }
    public required string ItemName { get; set; }
}