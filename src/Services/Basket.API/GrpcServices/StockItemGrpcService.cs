using Inventory.Grpc.Protos;
using ILogger = Serilog.ILogger;

namespace Basket.API.GrpcServices;

public class StockItemGrpcService
{
    private readonly StockProtoService.StockProtoServiceClient _stockProtoService;
    private readonly ILogger _logger;
    public StockItemGrpcService(StockProtoService.StockProtoServiceClient stockProtoService,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(stockProtoService);
        ArgumentNullException.ThrowIfNull(logger);

        _stockProtoService = stockProtoService;
        _logger = logger;
    }

    public async Task<StockModel> GetStock(string itemNo)
    {
        try
        {
            var stockItemRequest = new GetStockRequest { ItemNo = itemNo };
            var result = await _stockProtoService.GetStockAsync(stockItemRequest);
            _logger.Information("GetStock request for item no: {ItemNo}", itemNo);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while getting stock for item no: {ItemNo}", itemNo);
            return new StockModel(); 
        }
    }
}