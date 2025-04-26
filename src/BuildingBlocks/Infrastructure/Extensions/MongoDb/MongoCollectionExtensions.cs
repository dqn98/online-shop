using Infrastructure.Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Extensions;

public static class MongoCollectionExtensions
{
    public static Task<PagedList<TDestination>> PaginatedListAsync<TDestination>(
        this IMongoCollection<TDestination> collection, 
        FilterDefinition<TDestination> filterDefinition, 
        int pageIndex,
        int pageSize) where TDestination : class 
        => PagedList<TDestination>.ToPagedList(collection, filterDefinition, pageIndex, pageSize);
}