namespace Insig.Infrastructure.QueryBuilder;

public class SortColumn
{
    public string Column { get; }

    public bool IsAscending { get; }

    public SortColumn(string column, bool isAscending)
    {
        Column = column;
        IsAscending = isAscending;
    }

    public string Direction => IsAscending ? SortCriteria.Ascending : SortCriteria.Descending;
}
