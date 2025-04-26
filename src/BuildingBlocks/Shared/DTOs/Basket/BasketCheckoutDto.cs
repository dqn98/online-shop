namespace Shared.DTOs.Basket;

public class BasketCheckoutDto
{
    private string? _invoiceAddress;

    public string GetUserName() => _userName;

    private string _userName = string.Empty;

    public void SetUserName(string username)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));
        _userName = username;
    }

    public decimal TotalPrice { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string EmailAddress { get; set; }

    public string? ShippingAddress { get; set; }

    public string? InvoiceAddress
    {
        get => _invoiceAddress;
        set => _invoiceAddress = value ?? ShippingAddress;
    }
}