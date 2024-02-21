using System;
using System.Collections.Generic;

namespace Insig.Infrastructure.QueryBuilder;

public static class SortCriteria
{
    public static string Ascending => "ASC";
    public static string Descending => "DESC";

    public static SortColumn Parse(string sortCriteria)
    {
        var sortingData = sortCriteria.Split(",");

        if (sortingData is { Length: 2 })
        {
            throw new ArgumentException("Invalid sorting format.");
        }

        var columnName = sortingData[0].Trim();
        var isAscending = sortingData[1].Trim().ToUpperInvariant() == Ascending;

        return new SortColumn(columnName, isAscending);
    }

    public static SortColumn Parse(string columnName, string direction)
    {
        if (direction.Trim().ToUpperInvariant() != Ascending && direction.Trim().ToUpperInvariant() != Descending)
        {
            throw new ArgumentException("Invalid direction.");
        }

        var isAscending = direction.Trim().ToUpperInvariant() == Ascending;

        return new SortColumn(columnName, isAscending);
    }

    public static List<SortColumn> Parse(List<string> columnNames, List<string> directions)
    {
        var sortColumns = new List<SortColumn>();

        for (var i = 0; i < columnNames.Count; i++)
        {
            if (directions[i].Trim().ToUpperInvariant() != Ascending && directions[i].Trim().ToUpperInvariant() != Descending)
            {
                throw new ArgumentException("Invalid direction.");
            }

            var isAscending = directions[i].Trim().ToUpperInvariant() == Ascending;

            sortColumns.Add(new SortColumn(columnNames[i], isAscending));
        }

        return sortColumns;
    }
}