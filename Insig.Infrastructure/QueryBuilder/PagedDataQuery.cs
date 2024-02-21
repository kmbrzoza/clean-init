using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Insig.Common.Infrastructure.QueryBuilder;

namespace Insig.Infrastructure.QueryBuilder;

public class PagedDataQuery<T>
{
    private readonly IDbConnection _connection;

    public PagedDataQuery(IDbConnection connection, string query, string countQuery, DynamicParameters parameters, SearchCriteria searchCriteria)
    {
        _connection = connection;

        Query = query;
        CountQuery = countQuery;
        SearchCriteria = searchCriteria;
        Parameters = parameters;
    }

    public string Query { get; }
    public string CountQuery { get; }
    public SearchCriteria SearchCriteria { get; }

    public DynamicParameters Parameters { get; }

    public async Task<Page<T>> Execute()
    {
        var combinedQuery = string.Concat(CountQuery, Environment.NewLine, Query);

        var result = await _connection.QueryMultipleAsync(combinedQuery, Parameters);

        return new Page<T>
        {
            PageNumber = SearchCriteria.PageNumber,
            PageSize = SearchCriteria.PageSize,
            TotalCount = result.Read<int>().First(),
            Items = result.Read<T>().ToList()
        };
    }
}
