namespace Ordering.Domain.Exceptions;

public class EntityNotFoundException : ApplicationException
{
    public EntityNotFoundException(string entityName, object key) : base($"Entity {entityName} not found")
    {
        
    }
}