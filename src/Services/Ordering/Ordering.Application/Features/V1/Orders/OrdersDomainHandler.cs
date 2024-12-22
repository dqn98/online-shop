using MediatR;
using Ordering.Domain.OrderAggregate.Events;
using Serilog;

namespace Ordering.Application.Features.V1.Orders;

public class OrdersDomainHandler(ILogger logger) : INotificationHandler<OrderCreatedEvent>,
    INotificationHandler<OrderDeletedEvent>
{
    private ILogger _logger = logger;

    public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information($"Ordering Domain Event: {notification.GetType()}");
        return Task.CompletedTask;
    }

    public Task Handle(OrderDeletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.Information($"Ordering Domain Event: {notification.GetType()}");
        return Task.CompletedTask;
    }
}