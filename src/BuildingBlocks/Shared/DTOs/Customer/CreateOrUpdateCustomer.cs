namespace Shared.DTOs.Customer;

public class CreateOrUpdateCustomer
{
    public string? UserName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? EmailAddress { get; set; }
}