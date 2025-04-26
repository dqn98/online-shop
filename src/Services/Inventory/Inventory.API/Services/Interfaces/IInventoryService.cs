using Contracts.Domains.Interfaces;
using Inventory.API.Entities;
using Shared.DTOs.Inventory;
using Shared.SeedWork;

namespace Inventory.API.Services.Interfaces;

public interface IInventoryService : IMongoDbRepositoryBase<InventoryEntry>
{
    Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo, CancellationToken cancellationToken);
    Task<PagedList<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query, CancellationToken cancellationToken);
    Task<InventoryEntryDto?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto model, CancellationToken cancellationToken);
    Task<InventoryEntryDto> SalesItemAsync(string itemNo, SalesProductDto model, CancellationToken cancellationToken);
    Task DeleteByDocumentNo(string documentNo);

    Task<string> SaleOrderAsync(SalesOrderDto model);
}