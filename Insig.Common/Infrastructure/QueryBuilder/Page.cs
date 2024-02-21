using System.Collections.Generic;

namespace Insig.Common.Infrastructure.QueryBuilder;

public class Page<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public IList<T> Items { get; set; }

    public Page()
    {
        Items = new List<T>();
        TotalCount = 0;
    }
}