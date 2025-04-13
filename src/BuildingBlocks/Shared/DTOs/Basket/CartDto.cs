namespace Shared.DTOs.Basket;

public class CartDto
{
    public CartDto()
    {
    }
    
    public CartDto(string userName)
    {
        UserName = userName;
    }
    
    public string UserName { get; set; }
    public string EmailAddress { get; set; }
    public List<CardItemDto>? Items { get; set; } = new();
    public decimal? TotalPrice => Items?.Sum(x=> x.ItemPrice * x.Quantity);
}