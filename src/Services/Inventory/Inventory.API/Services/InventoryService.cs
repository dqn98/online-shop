﻿using AutoMapper;
using Infrastructure.Extensions;
using Inventory.API.Entities;
using Inventory.API.Extensions;
using Inventory.API.Repositories.Abstraction;
using Inventory.API.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.DTOs.Inventory;
using Shared.SeedWork;

namespace Inventory.API.Services;

public class InventoryService : MongoDbRepository<InventoryEntry>, IInventoryService
{
    private readonly IMapper _mapper;
    public InventoryService(IMongoClient client, DatabaseSettings databaseSettings, IMapper mapper) : base(client, databaseSettings)
    {
        ArgumentNullException.ThrowIfNull(mapper);
        _mapper = mapper;
    }

    public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo, CancellationToken cancellationToken)
    {
        var entities = await FindAll()
            .Find(x=>x.ItemNo.Equals(itemNo))
            .ToListAsync(cancellationToken);
        
        var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);
        return result;
    }

    public async Task<PagedList<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query, CancellationToken cancellationToken)
    {
        var filterSearchTerm = Builders<InventoryEntry>.Filter.Empty;
        var filterItemNo = Builders<InventoryEntry>.Filter.Eq(x => x.ItemNo, query.ItemNo());
        
        if(!string.IsNullOrEmpty(query.SearchTerm)) 
            filterSearchTerm = Builders<InventoryEntry>.Filter.Eq(x=>x.DocumentNo, query.SearchTerm);

        var andFilter = filterItemNo & filterSearchTerm;
        var pagedList = await Collection.PaginatedListAsync(
            filterDefinition: andFilter,
            pageIndex: query.PageIndex,
            pageSize: query.PageSize);

        var items = _mapper.Map<PagedList<InventoryEntryDto>>(pagedList);
        
        var result = new PagedList<InventoryEntryDto>(
            items, 
            totalItems: pagedList.GetMetaData().TotalItems, 
            pageIndex: query.PageIndex, 
            pageSize: query.PageSize);
        
        return result;
    }

    public async Task<InventoryEntryDto?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var filter = Builders<InventoryEntry>.Filter.Eq(x => x.Id, id);
        var entity = await Collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        var result = _mapper.Map<InventoryEntryDto>(entity);
        return result;
    }

    public async Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto model, CancellationToken cancellationToken)
    {
        var entity = new InventoryEntry(ObjectId.GenerateNewId().ToString())
        {
            Quantity = model.Quantity,
            ItemNo = model.ItemNo ?? string.Empty,
            DocumentType = model.DocumentType,
        };

        await CreateAsync(entity, cancellationToken: cancellationToken);
        var result = _mapper.Map<InventoryEntryDto>(entity);
        return result;
    }
}