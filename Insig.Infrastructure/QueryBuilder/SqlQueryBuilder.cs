using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using EnsureThat;
using Insig.Common.Infrastructure.QueryBuilder;

namespace Insig.Infrastructure.QueryBuilder;

public class SqlQueryBuilder
{
    private readonly IDbConnection _connection;

    private List<string> _columnsToSelect;
    private string _dataSource;
    private List<string> _whereConditions;
    private SqlQueryParameters _parameters;
    private SortColumn _sortingBy;
    private List<SortColumn> _sortingByColumns;

    private int? _topCount;

    private bool _isDistinct;

    public SqlQueryBuilder(IDbConnection connection)
    {
        _connection = connection;

        SqlMapper.RemoveTypeMap(typeof(DateTime));
        SqlMapper.AddTypeHandler(new DateTimeHandler());

        Reset();
    }

    public SqlQueryBuilder Select(params string[] columns)
    {
        EnsureArg.IsNotNull(columns, nameof(columns));

        var splitColumns = columns.Select(c => c.Trim());
        _columnsToSelect.AddRange(splitColumns);

        return this;
    }

    public SqlQueryBuilder SelectDistinct(params string[] columns)
    {
        _isDistinct = true;

        return Select(columns);
    }

    public SqlQueryBuilder SelectTop(int count, params string[] columns)
    {
        _topCount = count;

        return Select(columns);
    }

    public SqlQueryBuilder SelectAllProperties<T>(params string[] excludedColumns) where T : class
    {
        var properties = typeof(T).GetProperties();
        var columns = properties.Select(x => x.Name.Trim())
            .Except(excludedColumns);

        _columnsToSelect.AddRange(columns);

        return this;
    }

    public SqlQueryBuilder From(string dataSource)
    {
        Ensure.String.IsNotNullOrWhiteSpace(dataSource, nameof(dataSource));

        _dataSource = dataSource;

        return this;
    }

    public SqlQueryBuilder Where<T>(string column, T value) where T : struct
    {
        AddFilter(column, " = ", value);

        return this;
    }

    public SqlQueryBuilder Where<T>(string column, T? value) where T : struct
    {
        if (value == null)
        {
            return this;
        }

        AddFilter(column, " = ", value.Value);

        return this;
    }

    public SqlQueryBuilder Where(string column, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return this;
        }

        AddFilter(column, " = ", value);

        return this;
    }

    public SqlQueryBuilder Where(string column, object valueToFilter, SqlComparisonOperator comparisonOperator = SqlComparisonOperator.Equals)
    {
        Ensure.String.IsNotNullOrWhiteSpace(column, nameof(column));
        EnsureArg.IsNotNull(valueToFilter, nameof(valueToFilter));

        var paramName = _parameters.GetNextParameterName();

        switch (comparisonOperator)
        {
            case SqlComparisonOperator.Equals:
                _whereConditions.Add(string.Concat(column, " = ", paramName));
                break;
            case SqlComparisonOperator.Differs:
                _whereConditions.Add(string.Concat("( ", column, " <> ", paramName, " OR ", column, " IS NULL) "));
                break;
            case SqlComparisonOperator.Like:
                _whereConditions.Add(string.Concat(column, " LIKE '%' + ", paramName, " + '%'"));
                break;
            case SqlComparisonOperator.LessOrEqual:
                _whereConditions.Add(string.Concat(column, " <= ", paramName));
                break;
            case SqlComparisonOperator.GreaterOrEqual:
                _whereConditions.Add(string.Concat(column, " >= ", paramName));
                break;
            case SqlComparisonOperator.Less:
                _whereConditions.Add(string.Concat(column, " < ", paramName));
                break;
            case SqlComparisonOperator.Greater:
                _whereConditions.Add(string.Concat(column, " > ", paramName));
                break;
            default:
                throw new ArgumentException("Implement logic for new operator.");
        }

        _parameters.Add(paramName, valueToFilter);
        return this;
    }

    public SqlQueryBuilder WhereMultipleOrConditions(string[] columns, params ValueToFilter[] valueOperatorPair)
    {
        IEnumerable<string> innerConditions = GetColumnValueConditions(columns, valueOperatorPair);

        _whereConditions.Add(string.Concat("(", string.Join(" OR ", innerConditions), ")"));

        return this;
    }

    public SqlQueryBuilder WhereIn<T>(string column, List<T> values)
    {
        if (values == null || !values.Any())
        {
            return this;
        }

        var paramName = _parameters.GetNextParameterName();

        if (values.Count > 1)
        {
            _whereConditions.Add(string.Concat(column, " IN ", paramName));
        }
        else
        {
            _whereConditions.Add(string.Concat(column, " IN (", paramName, ")"));
        }

        _parameters.Add(paramName, values);

        return this;
    }

    public SqlQueryBuilder WhereIsNull(string column)
    {
        Ensure.String.IsNotNullOrWhiteSpace(column, nameof(column));

        _whereConditions.Add(string.Concat("( ", column, " IS NULL) "));

        return this;
    }

    public SqlQueryBuilder WhereIsNotNull(string column)
    {
        Ensure.String.IsNotNullOrWhiteSpace(column, nameof(column));

        _whereConditions.Add(string.Concat("( ", column, " IS NOT NULL) "));

        return this;
    }

    public SqlQueryBuilder WhenSpecified(int? value, Action<SqlQueryBuilder> actionToInvoke)
    {
        if (value.HasValue)
        {
            actionToInvoke(this);
        }

        return this;
    }

    public SqlQueryBuilder WhenSpecified(string value, Action<SqlQueryBuilder> actionToInvoke)
    {
        if (!string.IsNullOrEmpty(value))
        {
            actionToInvoke(this);
        }
        return this;
    }

    public SqlQueryBuilder WhenSpecified(DateTime? value, Action<SqlQueryBuilder> actionToInvoke)
    {
        if (value.HasValue)
        {
            actionToInvoke(this);
        }

        return this;
    }

    public SqlQueryBuilder When(bool applyFirstMethod, Action<SqlQueryBuilder> actionToInvokeWhenTrue, Action<SqlQueryBuilder> actionToInvokeWhenFalse = null)
    {
        if (applyFirstMethod)
        {
            actionToInvokeWhenTrue.Invoke(this);
        }
        else
        {
            actionToInvokeWhenFalse?.Invoke(this);
        }
        return this;
    }

    public SqlQueryBuilder OrderBy(List<string> columnNames, List<string> directions)
    {
        _sortingByColumns = SortCriteria.Parse(columnNames, directions);

        return this;
    }

    public SqlQueryBuilder OrderBy(string columnName, string direction)
    {
        _sortingBy = SortCriteria.Parse(columnName, direction);

        return this;
    }

    public SqlQueryBuilder OrderBy(string parameter)
    {
        if (parameter is not null)
        {
            var orderBy = parameter.Split(',');
            _sortingBy = SortCriteria.Parse(orderBy[0], orderBy[1]);
        }
        else
        {
            _sortingBy = SortCriteria.Parse("CreatedOn", "desc");
        }

        return this;
    }

    public DataQuery<T> BuildQuery<T>()
    {
        var selectQuery = BuildSelectQuery();
        var query = new DataQuery<T>(_connection, selectQuery, _parameters);

        Reset();
        return query;
    }

    public PagedDataQuery<T> BuildPagedQuery<T>(SearchCriteria searchCriteria)
    {
        Ensure.That(searchCriteria, nameof(searchCriteria)).IsNotNull();

        if (_sortingBy == null)
        {
            SetSortingBy();
        }

        var selectQuery = BuildSelectQuery(searchCriteria.PageNumber, searchCriteria.PageSize);
        var countQuery = BuildCountQuery();
        var query = new PagedDataQuery<T>(_connection, selectQuery, countQuery, _parameters, searchCriteria);

        Reset();
        return query;
    }

    private void SetSortingBy()
    {
        _sortingBy = new SortColumn(_columnsToSelect.First(x => !x.Contains(" AS ")), true);
    }

    private string BuildSelectQuery(int? pageNumber = null, int? pageSize = null)
    {
        var builder = new StringBuilder();
        builder.Append("SELECT ");

        if (_isDistinct)
        {
            builder.Append("DISTINCT ");
        }

        if (_topCount.HasValue)
        {
            builder.Append($"TOP {_topCount} ");
        }

        builder.Append(string.Join(", ", _columnsToSelect.Where(c => !string.IsNullOrEmpty(c))));

        builder.Append($" FROM {_dataSource} ");

        if (_whereConditions.Any())
        {
            builder.Append(" WHERE " + string.Join(" AND ", _whereConditions.Where(c => !string.IsNullOrEmpty(c))));
        }

        if (_sortingByColumns != null && _sortingByColumns.Count > 0)
        {
            var values = _sortingByColumns.Select(x => $"{x.Column} {x.Direction}");
            var sort = string.Join(", ", values);

            builder.Append($" ORDER BY {sort}");
        }
        else if (_sortingBy != null)
        {
            builder.Append($" ORDER BY {_sortingBy.Column} {_sortingBy.Direction}");
        }

        if (pageNumber != null && pageSize != null)
        {
            Ensure.That(pageSize.Value, nameof(pageSize)).IsGt(0);
            Ensure.That(pageNumber.Value, nameof(pageNumber)).IsGt(0);
            builder.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", (pageNumber - 1) * pageSize, pageSize);
        }

        return builder.ToString();
    }

    private string BuildCountQuery()
    {
        var builder = new StringBuilder();

        builder.Append("SELECT COUNT(*) FROM ");
        builder.Append(_dataSource);

        if (_whereConditions.Any())
        {
            builder.Append($" WHERE {string.Join(" AND ", _whereConditions)}");
        }

        return builder.ToString();
    }

    private void AddFilter(string column, string filterOperator, object valueToFilter)
    {
        Ensure.String.IsNotNullOrWhiteSpace(column, nameof(column));
        Ensure.String.IsNotNullOrWhiteSpace(filterOperator, nameof(filterOperator));
        EnsureArg.IsNotNull(valueToFilter, nameof(valueToFilter));

        var paramName = _parameters.GetNextParameterName();
        _parameters.Add(paramName, valueToFilter);

        _whereConditions.Add(string.Concat(column, filterOperator, paramName));
    }

    private void Reset()
    {
        _columnsToSelect = new List<string>();
        _dataSource = null;
        _whereConditions = new List<string>();

        _parameters = new SqlQueryParameters();
        _isDistinct = false;
        _sortingBy = null;
        _sortingByColumns = null;
    }

    private IEnumerable<string> GetColumnValueConditions(string[] columns, ValueToFilter[] valueOperatorPair)
    {
        for (int i = 0; i < valueOperatorPair.Length; ++i)
        {
            ValueToFilter pair = valueOperatorPair[i];

            var paramName = _parameters.GetNextParameterName();
            _parameters.Add(paramName, pair.Value);

            var clause = GetComparisonClause(columns[i], pair.ComparisonOperator, paramName);
            yield return clause;
        }
    }

    private string GetComparisonClause(string column, SqlComparisonOperator comparisonOperator, string paramName)
    {
        string result;
        switch (comparisonOperator)
        {
            case SqlComparisonOperator.Equals:
                result = string.Concat(column, " = ", paramName);
                break;
            case SqlComparisonOperator.Differs:
                result = string.Concat("( ", column, " <> ", paramName, " OR ", column, " IS NULL) ");
                break;
            case SqlComparisonOperator.Like:
                result = string.Concat(column, " LIKE '%' + ", paramName, " + '%'");
                break;
            case SqlComparisonOperator.LessOrEqual:
                result = string.Concat(column, " <= ", paramName);
                break;
            case SqlComparisonOperator.GreaterOrEqual:
                result = string.Concat(column, " >= ", paramName);
                break;
            case SqlComparisonOperator.IsNull:
                result = string.Concat("( ", column, " IS NULL) ");
                break;
            default:
                throw new ArgumentException("Implement logic for new operator.");
        }

        return result;
    }
}