using Inventory.API.Entities;
using Inventory.API.Repositories.Abstraction;
using Shared.DTOs.Inventory;

namespace Inventory.API.Services.Interfaces;

public interface IInventoryService : IMongoDbRepositoryBase<InventoryEntry>
{
    Task<IEnumerable<InventoryEntryDto>> GetAllByItemNo(string itemNo, CancellationToken cancellationToken);
    Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query, CancellationToken cancellationToken);
    Task<InventoryEntryDto> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto model, CancellationToken cancellationToken);
}