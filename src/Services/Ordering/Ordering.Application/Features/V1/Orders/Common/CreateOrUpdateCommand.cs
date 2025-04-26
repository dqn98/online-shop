using AutoMapper;
using Ordering.Application.Common.Mapping;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.V1.Orders;

public abstract class CreateOrUpdateCommand : IMapFrom<Order>
{
    public decimal TotalPrice { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }
    public string? ShippingAddress { get; set; }
    private string? _invoiceAddress;
    public string? InvoiceAddress 
    {
        get => _invoiceAddress;
        set => _invoiceAddress = string.IsNullOrWhiteSpace(value) ? ShippingAddress : value;
    }

    protected class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateOrUpdateCommand, Order>();
        }
    }
}