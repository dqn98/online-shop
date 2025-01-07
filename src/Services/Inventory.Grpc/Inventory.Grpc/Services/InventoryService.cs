using Grpc.Core;
using Inventory.Grpc.Protos;
using Inventory.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;
namespace Inventory.Grpc.Services;

public class InventoryService : StockProtoService.StockProtoServiceBase
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger _logger;
    
    public InventoryService(IInventoryRepository inventoryRepository, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(inventoryRepository);
        ArgumentNullException.ThrowIfNull(logger);
        
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }
    
    public override async Task<StockModel> GetStock(GetStockRequest request, ServerCallContext context)
    {
        _logger.Information("GetStock request for item no: {ItemNo}", request.ItemNo);
        var stockQuantity = await _inventoryRepository.GetStockQuantity(request.ItemNo);
        if (stockQuantity == null)
        {
            _logger.Error("Stock not found for item no: {ItemNo}", request.ItemNo);
            throw new RpcException(new Status(StatusCode.NotFound, "Stock not found"));
        }
        
        var result =  new StockModel
        {
            Quantity = stockQuantity ?? 0
        };
        _logger.Information("GetStock response for item no: {ItemNo}", request.ItemNo);
        return result;
    }
}