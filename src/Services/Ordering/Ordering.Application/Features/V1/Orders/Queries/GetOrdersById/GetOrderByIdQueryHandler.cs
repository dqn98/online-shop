using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, ApiResult<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger; 

    public GetOrderByIdQueryHandler(
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
    
    public async Task<ApiResult<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.Information("Begin: GetOrders request: {@Request}", request);
        try
        {
            var order = await _orderRepository.GetOrderByIdAsync(request.Id);
            var orderDto = _mapper.Map<OrderDto>(order);

            _logger.Information("End: GetOrders request: {@Request}", request);
            return new ApiSuccessResult<OrderDto>(orderDto);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while getting order by id: {@Request}", request);
            return new ApiErrorResult<OrderDto>(ex.Message);
        }
    }
}