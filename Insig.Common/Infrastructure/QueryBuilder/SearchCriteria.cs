namespace Insig.Common.Infrastructure.QueryBuilder;

public class SearchCriteria
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; } = 10;
    public string OrderBy { get; set; }

    public SearchCriteria()
    {
        PageNumber = 1;
    }

    public SearchCriteria(int pageNumber)
    {
        PageNumber = pageNumber;
    }

    public SearchCriteria(int pageNumber, int pageSize) : this(pageNumber)
    {
        PageSize = pageSize;
    }
}