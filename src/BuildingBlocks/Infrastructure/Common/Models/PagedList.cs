using MongoDB.Driver;
using Shared.SeedWork;

namespace Infrastructure.Common.Models;

public class PagedList<T> : List<T>
{
    public PagedList(IEnumerable<T> items, long totalItems, int pageIndex, int pageSize)
    {
        MetaData = new MetaData
        {
            TotalItems = totalItems,
            PageSize = pageSize,
            CurrentPage = pageIndex,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };

        AddRange(items);
    }
    
    public MetaData MetaData { get; }
    public MetaData GetMetaData() => MetaData;

    public static async Task<PagedList<T>> ToPagedList(
        IMongoCollection<T> source,
        FilterDefinition<T> filterDefinition,
        int pageIndex,
        int pageSize)
    {
        var count = await source.Find(filterDefinition).CountDocumentsAsync();
        var items = await source.Find(filterDefinition)
            .Skip((pageIndex - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
        
        return new PagedList<T>(items, count, pageIndex, pageSize);
    }
}