using Shared.DTOs.Order;
namespace Saga.Orchestrator.HttpRepository.Interfaces;

public interface IOrderHttpRepository
{
    Task<long> CreateOrder(CreateOrderDto createOrderDto);
    Task<OrderDto?> GetOrder(long id);
    Task<bool> DeleteOrder(long id);
    Task DeleteOrderByDocumentNo(string documentNo);
}