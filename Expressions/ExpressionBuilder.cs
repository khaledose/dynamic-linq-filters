using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Expressions;

public static class ExpressionBuilder
{
    private static readonly ConcurrentDictionary<string, Delegate> _compiledExpressions = new();

    public static Func<T, object> BuildGroupingExpression<T>(IEnumerable<string> propertyNames)
    {
        // Create a cache key based on the type and property names
        var cacheKey = $"{typeof(T).FullName}_{string.Join("_", propertyNames)}";

        // Create or retrieve a key selector function from the cache
        var keySelector = (Func<T, object>)_compiledExpressions.GetOrAdd(cacheKey, _ =>
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            // Retrieve property access expressions for the specified property names
            var properties = propertyNames
                .Select(name => Expression.Property(parameter, name))
                .ToArray();

            // Dynamically create a ValueTuple with the property values
            var tupleType = Type.GetType($"System.ValueTuple`{properties.Length}")!
                .MakeGenericType(properties.Select(p => p.Type).ToArray());
            var newTuple = Expression.New(tupleType.GetConstructor(properties.Select(p => p.Type).ToArray())!, properties);

            // Compile the lambda expression to return the ValueTuple
            return Expression.Lambda<Func<T, object>>(Expression.Convert(newTuple, typeof(object)), parameter).Compile();
        });

        return keySelector;
    }

    private static readonly ConcurrentDictionary<string, Delegate> _compiledFilterExpressions = new();

    public static Func<T, bool> BuildFilterExpression<T>(Dictionary<string, IEnumerable<object>> filters)
    {
        // Create a cache key based on the type and filter properties
        var cacheKey = $"{typeof(T).FullName}_{string.Join("_", filters.Keys)}";

        // Create or retrieve a filter predicate function from the cache
        var predicate = (Func<T, bool>)_compiledFilterExpressions.GetOrAdd(cacheKey, _ =>
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            // Build filtering conditions for each column and its values
            var conditions = filters.Select(filter =>
            {
                // Get the property to filter on
                var property = Expression.Property(parameter, filter.Key);

                // Convert filter values to match the property's type
                var propertyType = property.Type;
                var filterValues = filter.Value.Select(value =>
                    Expression.Constant(Convert.ChangeType(value, propertyType)));

                // Build a condition: property == value1 || property == value2 || ...
                var equalityConditions = filterValues
                    .Select(value => Expression.Equal(property, value))
                    .Aggregate(Expression.OrElse);

                return equalityConditions;
            });

            // Combine all conditions using AND: (cond1) && (cond2) && ...
            var combinedCondition = conditions.Aggregate(Expression.AndAlso);

            // Compile the lambda expression into a function
            return Expression.Lambda<Func<T, bool>>(combinedCondition, parameter).Compile();
        });

        // Apply the compiled filter predicate to the source
        return predicate;
    }
}
