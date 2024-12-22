using Contracts.Common.Events;
using Contracts.Common.Interfaces;
using Infrastructure.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Infrastructure.Extensions;

public static class MediatorExtensions 
{
    public static async Task DispatchDomainEventsAsync(
        this IMediator mediator, 
        List<BaseEvent> domainEvents, 
        ILogger logger)
    {
        foreach (var domainEvent in domainEvents)
        {
            try
            {
                await mediator.Publish(domainEvent);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurred while publishing domain events.");
            }

            var data = new SerializeService().Serialize(domainEvent);
            logger.Information("Published domain event: {DomainEvent}", data);
            
        }
    }
}