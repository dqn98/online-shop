using Contracts.Common.Interfaces;
using Contracts.Domains;

namespace Contracts.Common.Events;

public class EventEntity<T> : EntityBase<T>, IEventEntity<T>
{
    private readonly List<BaseEvent> _domainEvents = new();
    public void AddDomainEvent(BaseEvent baseEvent)
    {
        _domainEvents.Add(baseEvent);
    }

    public void RemoveDomainEvent(BaseEvent baseEvent)
    {
        _domainEvents.Remove(baseEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public IReadOnlyCollection<BaseEvent> DomainEvents() => _domainEvents.AsReadOnly();
}