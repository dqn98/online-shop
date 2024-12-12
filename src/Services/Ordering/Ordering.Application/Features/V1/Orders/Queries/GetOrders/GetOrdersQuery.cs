using MediatR;
using Ordering.Application.Common.Models;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrdersQuery : IRequest<ApiResult<List<OrderDto>>>
{
    public string Username { get; private set; }

    public GetOrdersQuery(string username)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentNullException(nameof(username));
    }
}