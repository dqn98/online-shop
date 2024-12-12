namespace Ordering.Domain.Exceptions;

public class InvalidEntityTypeException(string entityName, object key)
    : ApplicationException($"Entity {entityName} with key {key} is not valid.");