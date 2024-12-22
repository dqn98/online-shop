using AutoMapper;
using EventBus.Messages.IntegrationEvents.Events;
using MediatR;
using Ordering.Application.Common.Mapping;
using Ordering.Domain.Entities;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class CreateOrderCommand : CreateOrUpdateCommand, IRequest<ApiResult<long>>, IMapFrom<Order>,
    IMapFrom<BasketCheckoutEvent>
{
    public string? UserName { get; set; }
    
    public new class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateOrderCommand, Order>();
            CreateMap<BasketCheckoutEvent, CreateOrderCommand>();
        }
    }
}