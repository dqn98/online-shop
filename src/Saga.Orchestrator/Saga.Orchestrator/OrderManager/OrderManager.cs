using AutoMapper;
using Contracts.Sagas.OrderManager;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.DTOs.Basket;
using Shared.DTOs.Inventory;
using Shared.DTOs.Order;
using Ilogger = Serilog.ILogger;

namespace Saga.Orchestrator.OrderManager;

public class OrderManager : ISagaOrderManager<BasketCheckoutDto, OrderResponse>
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IInventoryHttpRepository _inventoryHttpRepository;
    private readonly IMapper _mapper;
    private readonly Ilogger _logger;

    public OrderManager(
        IOrderHttpRepository orderHttpRepository,
        IBasketHttpRepository basketHttpRepository,
        IInventoryHttpRepository inventoryHttpRepository,
        IMapper mapper,
        Ilogger logger)
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
    
    public OrderResponse CreateOrder(BasketCheckoutDto input)
    {
        var orderStateMachine =
            new Stateless.StateMachine<EOrderTransactionState, EOrderAction>(EOrderTransactionState.NotStarted);

        long orderId = -1;
        CartDto? cart = null;
        OrderDto? addedOrder = null;
        string? inventoryDocumentNo = string.Empty;

        orderStateMachine.Configure(EOrderTransactionState.NotStarted)
            .PermitDynamic(EOrderAction.GetBasket, () =>
            {
                var username = input.GetUserName();
                if (string.IsNullOrEmpty(username)) return EOrderTransactionState.BasketGotFailed;

                cart = _basketHttpRepository.GetBasket(username).Result;

                return cart != null ? EOrderTransactionState.BasketGot : EOrderTransactionState.BasketGotFailed;
            });

        orderStateMachine.Configure(EOrderTransactionState.BasketGot)
            .PermitDynamic(EOrderAction.CreateOrder, () =>
            {
                if (cart == null)
                {
                    _logger.Error("Cart is null");
                    return EOrderTransactionState.OrderCreateFailed;
                }
                input.TotalPrice = cart.TotalPrice;
                var order = _mapper.Map<CreateOrderDto>(input);
                orderId = _orderHttpRepository.CreateOrder(order).Result;

                return orderId > 0 ? EOrderTransactionState.OrderCreated : EOrderTransactionState.OrderCreateFailed;
            }).OnEntry(() => orderStateMachine.Fire(EOrderAction.CreateOrder));

        orderStateMachine.Configure(EOrderTransactionState.OrderCreated)
            .PermitDynamic(EOrderAction.GetOrder, () =>
            {
                addedOrder = _orderHttpRepository.GetOrder(orderId).Result;
                return addedOrder != null ? EOrderTransactionState.OrderGot : EOrderTransactionState.OrderGetFailed;
            }).OnEntry(() => orderStateMachine.Fire(EOrderAction.GetOrder));

        orderStateMachine.Configure(EOrderTransactionState.OrderGot)
            .PermitDynamic(EOrderAction.UpdateInventory, () =>
            {
                if (addedOrder != null && cart != null)
                {
                    var salesOrder = new SalesOrderDto()
                    {
                        OrderNo = addedOrder.DocumentNo,
                        SaleItems = _mapper.Map<List<SaleItemDto>>(cart.Items)
                    };
                                            
                    inventoryDocumentNo = _inventoryHttpRepository.CreateOrderSale(addedOrder.DocumentNo, salesOrder).Result;
                    return !string.IsNullOrEmpty(inventoryDocumentNo)
                        ? EOrderTransactionState.InventoryUpdated
                        : EOrderTransactionState.InventoryUpdateFailed;
                }
                
                return EOrderTransactionState.InventoryUpdateFailed;
            }).OnEntry(() => orderStateMachine.Fire(EOrderAction.UpdateInventory));

        orderStateMachine.Configure(EOrderTransactionState.InventoryUpdated)
            .PermitDynamic(EOrderAction.DeleteBasket, () =>
            {
                var username = input.GetUserName();
                if (string.IsNullOrEmpty(username))
                {
                    _logger.Error("Username is null");
                    return EOrderTransactionState.InventoryUpdateFailed;
                }
                var result = !string.IsNullOrEmpty(input.GetUserName()) && _basketHttpRepository.DeleteBasket(username).Result;
                
                return result ? EOrderTransactionState.BasketDeleted : EOrderTransactionState.InventoryUpdateFailed;
            }).OnEntry(() => orderStateMachine.Fire(EOrderAction.DeleteBasket));

        orderStateMachine.Configure(EOrderTransactionState.InventoryUpdateFailed)
            .PermitDynamic(EOrderAction.DeleteInventory, () =>
            {
                RollbackOrder(input.GetUserName(), inventoryDocumentNo, orderId);
                return EOrderTransactionState.InventoryRollback;
            });
        
        orderStateMachine.Fire(EOrderAction.GetBasket);
        
        return new OrderResponse(orderStateMachine.State == EOrderTransactionState.InventoryUpdated);
    }

    public OrderResponse RollbackOrder(string username, string documentNo, long orderId)
    {
        return new OrderResponse(true);
    }
}