using EventBus.Messages.IntegrationEvents.Interfaces.Basket;

namespace EventBus.Messages.IntegrationEvents.Events.Basket;

public record BasketCheckoutEvent() : IntegrationBaseEvent, IBasketCheckoutEvent
{
    public required string UserName { get; set; }
    public decimal TotalPrice { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string EmailAddress { get; set; }
    public required string ShippingAddress { get; set; }
    public required string InvoiceAddress { get; set; }
}