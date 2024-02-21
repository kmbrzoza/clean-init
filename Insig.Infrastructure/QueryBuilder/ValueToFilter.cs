namespace Insig.Infrastructure.QueryBuilder;

public class ValueToFilter
{
    public object Value { get; set; }
    public SqlComparisonOperator ComparisonOperator { get; set; }

    public ValueToFilter(SqlComparisonOperator comparisonOperator, object value = null)
    {
        Value = value;
        ComparisonOperator = comparisonOperator;
    }

    public static ValueToFilter IsEqualTo(object value)
    {
        return new ValueToFilter(SqlComparisonOperator.Equals, value);
    }

    public static ValueToFilter IsLike(object value)
    {
        return new ValueToFilter(SqlComparisonOperator.Like, value);
    }

    public static ValueToFilter IsNotEqualTo(object value)
    {
        return new ValueToFilter(SqlComparisonOperator.Differs, value);
    }

    public static ValueToFilter IsLessOrEqualTo(object value)
    {
        return new ValueToFilter(SqlComparisonOperator.LessOrEqual, value);
    }

    public static ValueToFilter IsGreaterOrEqualTo(object value)
    {
        return new ValueToFilter(SqlComparisonOperator.GreaterOrEqual, value);
    }

    public static ValueToFilter IsDiffers(object value)
    {
        return new ValueToFilter(SqlComparisonOperator.Differs, value);
    }

    public static ValueToFilter IsNull()
    {
        return new ValueToFilter(SqlComparisonOperator.IsNull);
    }
}