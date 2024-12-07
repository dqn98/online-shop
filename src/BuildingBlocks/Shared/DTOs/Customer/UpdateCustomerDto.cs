namespace Shared.DTOs.Customer;

public class UpdateCustomerDto : CreateOrUpdateCustomer
{
    public int Id { get; set; }
}