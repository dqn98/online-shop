using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, ApiResult<List<OrderDto>>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger; 

    public GetOrdersQueryHandler(
        IOrderRepository orderRepository, 
        IMapper mapper,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(orderRepository, nameof(orderRepository));
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        
        _orderRepository = orderRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<ApiResult<List<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        _logger.Information("Begin: GetOrders request: {@Request}", request);
        var orderEntities = await _orderRepository.GetOrdersByUserNameAsync(request.UserName);
        var orderList = _mapper.Map<List<OrderDto>>(orderEntities);
        
        _logger.Information("End: GetOrders request: {@Request}", request);
        return new ApiSuccessResult<List<OrderDto>>(orderList);
    }
}