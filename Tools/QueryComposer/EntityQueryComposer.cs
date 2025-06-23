using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace BookTracker.Tools.QueryComposer;

public class EntityQueryComposer
{
    private readonly ConcurrentDictionary<string, LambdaExpression> _sortCache = new();

    private Expression<Func<T, bool>> ComposeFilterExpression<T>(Filter[] filters)
    {
        if (filters == null || filters.Length == 0)
            return x => true;

        var objectParameter = Expression.Parameter(typeof(T), "obj");
        Expression? finalExpression = null;

        foreach (var filter in filters)
        {
            var (propertyAccess, propertyType) = PathToProperty<T>(filter.PropertyName, objectParameter);

            var constantValue = Convert.ChangeType(filter.Value, Nullable.GetUnderlyingType(propertyType) ?? propertyType);
            if (constantValue is string strValue)
            {
                constantValue = strValue.ToLower();
            }
            var constant = Expression.Constant(constantValue, propertyType);

            Expression comparison = filter.Operator switch
            {
                FilterOperator.Equals => Expression.Equal(propertyAccess, constant),
                FilterOperator.NotEquals => Expression.NotEqual(propertyAccess, constant),
                FilterOperator.GreaterThan => Expression.GreaterThan(propertyAccess, constant),
                FilterOperator.LessThan => Expression.LessThan(propertyAccess, constant),
                FilterOperator.Contains when propertyType == typeof(string) =>

                    Expression.Call(
                        Expression.Call(propertyAccess, typeof(string).GetMethod("ToLower", [])!),
                            typeof(string).GetMethod("Contains", new[] { typeof(string) })!, constant),
                _ => throw new NotSupportedException($"Operator {filter.Operator} is not supported for property type {propertyType}")
            };

            finalExpression = finalExpression == null
                ? comparison
                : Expression.AndAlso(finalExpression, comparison);
        }

        return Expression.Lambda<Func<T, bool>>(finalExpression!, objectParameter);
    }

    private Tuple<Expression, Type> PathToProperty<T>(string propertyName, Expression from)
    {
        var components = propertyName.Split('.');

        PropertyInfo? property = null;
        Expression propertyAccess = from;
        Type currentType = typeof(T);

        for (int i = 0; i < components.Length; i++)
        {
            property = GetPropertyInfo(currentType, components[i]);
            propertyAccess = Expression.Property(propertyAccess, property);
            currentType = property.PropertyType;
        }

        return new Tuple<Expression, Type>(propertyAccess, currentType);
    }

    public IQueryable<T> Filter<T>(IQueryable<T> query, params Filter[] filters)
    {
        if (filters == null || filters.Length == 0)
            return query;

        var filterExpression = ComposeFilterExpression<T>(filters);
        return query.Where(filterExpression);
    }

    private Expression<Func<T, object>> ComposeSortExpression<T>(Sort sortQuery)
    {
        string cacheKey = $"{typeof(T).FullName}.Sort.{sortQuery.SortBy}";

        if (_sortCache.TryGetValue(cacheKey, out var cached))
            return (Expression<Func<T, object>>)cached;

        var parameter = Expression.Parameter(typeof(T), "obj");

        var (propertyAccess, propertyType) = PathToProperty<T>(sortQuery.SortBy, parameter);

        Expression conversion = Expression.Convert(propertyAccess, typeof(object));

        var lambda = Expression.Lambda<Func<T, object>>(conversion, parameter);
        _sortCache.TryAdd(cacheKey, lambda);
        return lambda;
    }

    public IQueryable<T> Sort<T>(IQueryable<T> query, params Sort[] sortQueries)
    {
        if (sortQueries == null || sortQueries.Length == 0)
            return query;

        var firstSortQuery = sortQueries[0];
        var firstSortQueryExpression = ComposeSortExpression<T>(firstSortQuery);
        IOrderedQueryable<T> orderedQuery = firstSortQuery.SortDirection == SortDirection.Ascending
            ? query.OrderBy(firstSortQueryExpression)
            : query.OrderByDescending(firstSortQueryExpression);

        for (var i = 1; i < sortQueries.Length; i++)
        {
            var sortQuery = sortQueries[i];
            var sortExpression = ComposeSortExpression<T>(sortQuery);
            orderedQuery = sortQuery.SortDirection == SortDirection.Ascending
                ? orderedQuery.ThenBy(sortExpression)
                : orderedQuery.ThenByDescending(sortExpression);
        }

        return orderedQuery;
    }

    public IQueryable<T> FilterAndSort<T>(IQueryable<T> query, Filter[]? filters, Sort[]? sortQueries)
    {
        var filteredQuery = Filter(query, filters ?? []);
        return Sort(filteredQuery, sortQueries ?? []);
    }


    private static PropertyInfo GetPropertyInfo<T>(string propertyName)
    {
        return GetPropertyInfo(typeof(T), propertyName);
    }

    private static PropertyInfo GetPropertyInfo(Type fromType, string propertyName)
    {
        var property = fromType.GetProperty(propertyName,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
            throw new ArgumentException($"Property '{propertyName}' does not exist on type {fromType.Name}");

        return property;
    }
}