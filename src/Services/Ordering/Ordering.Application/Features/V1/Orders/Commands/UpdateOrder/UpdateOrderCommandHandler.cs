using AutoMapper;
using MediatR;
using Ordering.Application.Common.Exceptions;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Ordering.Domain.Entities;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, ApiResult<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    
    public UpdateOrderCommandHandler(
        IOrderRepository orderRepository, 
        ILogger logger,
        IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(orderRepository, nameof(orderRepository));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentNullException.ThrowIfNull(mapper, nameof(mapper));
        
        _orderRepository = orderRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ApiResult<OrderDto>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.Information($"BEGIN: {nameof(UpdateOrderCommandHandler)} - Order: {request.Id}");
        var orderEntity = await _orderRepository.GetByIdAsync(request.Id);
        if(orderEntity == null) throw new NotFoundException(nameof(Order), request.Id);
        
        orderEntity = _mapper.Map(request, orderEntity);
        var updatedOrder = await _orderRepository.UpdateAsync(orderEntity);
        
        _orderRepository.SaveChangesAsync();
        _logger.Information($"Order {request.Id} was successfully updated.");
        var result = _mapper.Map<OrderDto>(updatedOrder);
        
        _logger.Information($"END: {nameof(UpdateOrderCommandHandler)} - Order: {orderEntity.Id}");

        return new ApiSuccessResult<OrderDto>(result);
    }
}