﻿using MediatR;
using Microsoft.Extensions.Logging;

namespace Ordering.Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        _logger = logger;
    }


    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            _logger.LogError(ex, "Application unhandled exception for request: {Name} ({@Request}", requestName, request);
            throw;
        }
    }
}