﻿namespace Ordering.Application.Common.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException() : base() { }
    
    public NotFoundException(string message) 
        : base(message) { }
    
    public NotFoundException(string message, Exception inner) 
        : base(message, inner) { }
    
    public NotFoundException(string name, object key) 
        : base($"Entity \"{name}\" ({key}) was not found.") { }
}