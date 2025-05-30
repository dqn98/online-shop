﻿namespace Shared.SeedWork;

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

    private MetaData MetaData { get; }

    public MetaData GetMetaData()
    {
        return MetaData;
    }
}