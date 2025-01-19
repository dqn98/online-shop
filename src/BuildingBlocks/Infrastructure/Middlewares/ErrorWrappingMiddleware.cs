using System.Net;
using Contracts.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using ILogger = Serilog.ILogger;
namespace Infrastructure.Middlewares;

public class ErrorWrappingMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;
    private readonly ISerializeService _serializeService;
    
    public ErrorWrappingMiddleware(
        RequestDelegate next, 
        ILogger logger,
        ISerializeService serializeService)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(serializeService);
        
        _next = next;
        _logger = logger;
        _serializeService = serializeService;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ex.Message);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(_serializeService.Serialize(new
            {
                StatusCode = context.Response.StatusCode,
                Message = ex.Message
            }));
        }
    }
}