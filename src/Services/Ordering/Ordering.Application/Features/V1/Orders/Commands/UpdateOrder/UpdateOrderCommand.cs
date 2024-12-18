using AutoMapper;
using Infrastructure.Mapping;
using MediatR;
using Ordering.Application.Common.Mapping;
using Ordering.Application.Common.Models;
using Ordering.Domain.Entities;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders.Commands.UpdateOrder;

public class UpdateOrderCommand : CreateOrUpdateCommand, IRequest<ApiResult<OrderDto>>, IMapFrom<Order>
{
    public long Id { get; private set; }
    
    public void SetId(long id) => Id = id;

    public void MappingProfile(Profile profile) => profile.CreateMap<UpdateOrderCommand, Order>()
        .ForMember(dest => dest.Status, opts => opts.Ignore())
        .IgnoreAllNonExisting();
    
}