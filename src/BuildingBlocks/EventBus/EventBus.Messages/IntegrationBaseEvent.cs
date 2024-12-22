using MediatR;

namespace EventBus.Messages;

public abstract record IntegrationBaseEvent() : IIntegrationEvent
{
    public DateTime CreationDate { get; } = DateTime.UtcNow;
    
    public Guid Id { get; set; }
}