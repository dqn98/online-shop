using AutoMapper;
using Ordering.Application.Common.Mapping;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.V1.Orders;

public class CreateOrUpdateCommand : IMapFrom<Order>
{
    public decimal TotalPrice { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string EmailAddress { get; set; }
    public required string ShippingAddress { get; set; }
    public string InvoiceAddress { get; set; } = string.Empty;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateOrUpdateCommand, Order>();
    }
}