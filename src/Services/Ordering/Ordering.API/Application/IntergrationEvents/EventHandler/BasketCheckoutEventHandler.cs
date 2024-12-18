using AutoMapper;
using EventBus.Messages.IntegrationEvents.Events.Basket;
using MassTransit;
using MediatR;
using ILogger = Serilog.ILogger;

namespace Ordering.API.Application.IntergrationEvents.EventHandler;

public class BasketCheckoutEventHandler : IConsumer<BasketCheckoutEvent>
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public BasketCheckoutEventHandler(
        IMediator mediator, 
        IMapper mapper, 
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(logger);
        
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }
    
    private static string MethodName => nameof(BasketCheckoutEventHandler);
    
    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        _logger.Information($"Begin method {MethodName}");
        
        var command = _mapper.Map<BasketCheckoutEvent>(context.Message);
        var result = await _mediator.Send(command);
        _logger.Information($"BasketCheckoutEvent consumed successfully. " +
                            $"Order is created with Id: {result}");
        _logger.Information($"End method {MethodName}");
    }
}