using System.Threading.Tasks;
using Infrastructure.Common.Repositories;
using Inventory.Grpc.Entities;
using Inventory.Grpc.Repositories.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Shared.Configurations;

namespace Inventory.Grpc.Repositories;

public class InventoryRepository(IMongoClient client, MongoDbSettings databaseSettings)
    : MongoDbRepository<InventoryEntry>(client, databaseSettings), IInventoryRepository
{
    public async Task<double?> GetStockQuantity(string itemNo)
        => await Collection.AsQueryable()
            .Where(x => x.ItemNo == itemNo)
            .SumAsync(x=>x.Quantity);
}