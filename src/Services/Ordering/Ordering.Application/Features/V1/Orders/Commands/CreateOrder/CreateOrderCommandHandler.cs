using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Shared.SeedWork;
using ILogger = Serilog.ILogger;

namespace Ordering.Application.Features.V1.Orders;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiResult<long>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository, 
        ILogger logger, 
        IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(orderRepository, nameof(orderRepository));
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        
        _orderRepository = orderRepository;
        _logger = logger;
        _mapper = mapper;
    }

    private const string MethodName = nameof(CreateOrderCommandHandler);


    public async Task<ApiResult<long>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.Information($"Beginning command {MethodName} - Username: {request.UserName}");
        
        var orderEntity = _mapper.Map<Order>(request);
        var addedOrder = await _orderRepository.CreateOrderAsync(orderEntity);
        await _orderRepository.SaveChangesAsync();
        
        _logger.Information($"Order: {addedOrder.Id} has been created");
        
        return new ApiSuccessResult<long>(addedOrder.Id);
    }
}