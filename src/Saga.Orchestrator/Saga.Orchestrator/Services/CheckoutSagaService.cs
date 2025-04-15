using AutoMapper;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOs.Basket;
using Shared.DTOs.Inventory;
using Shared.DTOs.Order;
using ILogger = Serilog.ILogger;

namespace Saga.Orchestrator.Services;

public class CheckoutSagaService : ICheckoutSagaService
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IInventoryHttpRepository _inventoryHttpRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public CheckoutSagaService(
        IOrderHttpRepository orderHttpRepository,
        IBasketHttpRepository basketHttpRepository,
        IInventoryHttpRepository inventoryHttpRepository,
        IMapper mapper,
        ILogger logger)
    {
        
        ArgumentException.ThrowIfNullOrEmpty(nameof(orderHttpRepository));
        ArgumentException.ThrowIfNullOrEmpty(nameof(basketHttpRepository));
        ArgumentException.ThrowIfNullOrEmpty(nameof(inventoryHttpRepository));
        ArgumentException.ThrowIfNullOrEmpty(nameof(mapper));
        ArgumentException.ThrowIfNullOrEmpty(nameof(logger));
        
        _orderHttpRepository = orderHttpRepository;
        _basketHttpRepository = basketHttpRepository;
        _inventoryHttpRepository = inventoryHttpRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<bool> CheckoutOrder(string username, BasketCheckoutDto basketCheckout)
    {
        // Get cart from basketHttpRepository
        
        _logger.Information("Start: Get cart from basketHttpRepository");
        var cart = await _basketHttpRepository.GetBasket(username);
        if (cart == null)
        {
            return false;
        }
        
        _logger.Information("End: Get cart from basketHttpRepository successfully");
        
        _logger.Information("Start: Create order");
        var order = _mapper.Map<CreateOrderDto>(basketCheckout);
        order.TotalPrice = cart.TotalPrice;
        _logger.Information("End: Create order");
        var orderId = await _orderHttpRepository.CreateOrder(order);
        if (orderId <= 0)
        {
            _logger.Error("Error: Order creation failed");
            return false;
        }
        
        var addedOrder = await _orderHttpRepository.GetOrder(orderId);

        if (addedOrder == null)
        {
            _logger.Error("Error: Can not get order");
            return false;
        }
        
        _logger.Information("Information: Created order successfully, orderId: {OrderId} - DocumentNo: {DocumentNo}", 
            orderId, addedOrder.DocumentNo);
        
        bool result = false;
        var inventoryDocumentNos = new List<string>();
        try
        {
            // Sales Item from InventoryHttpRepository
            foreach (var item in cart.Items)
            {
                _logger.Information($"Start: Sale item {item.ItemNo} - DocumentNo: {addedOrder.DocumentNo} - Quantity: {item.Quantity} from inventoryHttpRepository");

                var saleOrder = new SalesProductDto(addedOrder.DocumentNo, item.Quantity);
                saleOrder.SetItemNo(item.ItemNo);
                
                var documentNo = await _inventoryHttpRepository.CreateSalesOrder(saleOrder);
                if (!string.IsNullOrWhiteSpace(documentNo))
                {
                    inventoryDocumentNos.Add(documentNo);
                }
                
                _logger.Information($"End: Sale item {item.ItemNo} - DocumentNo: {addedOrder.DocumentNo} - Quantity: {item.Quantity} from inventoryHttpRepository successfully");
            }

            result = await _basketHttpRepository.DeleteBasket(username);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error: Order creation failed. RollBackCheckoutOrder");
            // Rollback order
            
            await RollBackCheckoutOrder(username, addedOrder.Id, inventoryDocumentNos);
            result = false;
        }
        return result;
    }

    private async Task RollBackCheckoutOrder(string username, long orderId, List<string> inventoryDocumentNos)
    {
        _logger.Information($"Start: RollBackCheckoutOrder - Username: {username} - OrderId: {orderId}, DocumentNos: {string.Join(", ", inventoryDocumentNos)}");
        
        // Delete order by orderId
        var deletedDocumentNos = new List<string>();
        foreach (var documentNo in inventoryDocumentNos)
        {
            _logger.Information($"Start: Rollback order - DocumentNo: {documentNo}");
            await _inventoryHttpRepository.DeleteOrderByDocumentNo(documentNo);
            deletedDocumentNos.Add(documentNo);
            _logger.Information($"End: Rollback order - DocumentNo: {documentNo} successfully");
        }
        
        _logger.Information($"End: Rollback order - DocumentNos: {string.Join(", ", deletedDocumentNos)} successfully");
    }
}