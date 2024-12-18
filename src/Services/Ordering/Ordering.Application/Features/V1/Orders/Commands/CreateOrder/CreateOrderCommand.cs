using AutoMapper;
using EventBus.Messages.IntegrationEvents.Events.Basket;
using MediatR;
using Ordering.Application.Common.Mapping;
using Ordering.Application.Common.Models;
using Ordering.Domain.Entities;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class CreateOrderCommand : CreateOrUpdateCommand, 
    IRequest<ApiResult<long>>, IMapFrom<Order>, IMapFrom<BasketCheckoutEvent>
{
    public required string UserName { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateOrderCommand, Order>();
        profile.CreateMap<BasketCheckoutEvent, CreateOrUpdateCommand>();
    }
}