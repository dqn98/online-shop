using Inventory.API.Entities;
using Inventory.API.Extensions;
using MongoDB.Driver;
using Shared.Configurations;
using Shared.Enums.Inventory;

namespace Inventory.API.Persistences;

public class InventoryDbSeed
{
    public async Task SeedDataAsync(IMongoClient mongoClient, MongoDbSettings databaseSettings)
    {
        var databaseName = databaseSettings.DatabaseName;
        var database = mongoClient.GetDatabase(databaseName);
        var inventoryCollection = database.GetCollection<InventoryEntry>("InventoryEntries");
        if (await inventoryCollection.EstimatedDocumentCountAsync() == 0)
        {
            await inventoryCollection.InsertManyAsync(GetPreconfiguredInventoryEntries());
        }
    }
    
    private IEnumerable<InventoryEntry> GetPreconfiguredInventoryEntries()
    {
        return new List<InventoryEntry>
        {
            new InventoryEntry
            {
                DocumentNo = Guid.NewGuid().ToString(),
                ItemNo = "Lotus",
                ExternalDocumentNo = Guid.NewGuid().ToString(),
                Quantity = 10,
                DocumentType = EDocumentType.Purchase
            },
            new InventoryEntry
            {
                DocumentNo = Guid.NewGuid().ToString(),
                ItemNo = "Cadillac",
                ExternalDocumentNo = Guid.NewGuid().ToString(),
                Quantity = 10,
                DocumentType = EDocumentType.Purchase
            }
        };
    }
}