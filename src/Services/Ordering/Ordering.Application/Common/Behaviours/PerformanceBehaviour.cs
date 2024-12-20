﻿using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ordering.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TResponse> _logger;

    public PerformanceBehaviour(ILogger<TResponse> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        
        _timer = new Stopwatch();
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();
        var response = await next();
        _timer.Stop();
        var elapsedMilliseconds = _timer.ElapsedMilliseconds;
        if (elapsedMilliseconds <= 500)
        {
            return response;
        }
        
        var requestName = typeof(TRequest).Name;
        _logger.LogWarning("Application long running request: {Name} ({ElapsedMilliseconds} milliseconds)"
            , requestName, elapsedMilliseconds);
        
        return response;
    }
}