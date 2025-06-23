using System;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;

namespace BookTracker.Tools;

public static class ReflectionHelper
{
    public static Func<object, object?> CreateGetter(PropertyInfo propertyInfo)
    {
        var objParam = Expression.Parameter(typeof(object), "obj");
        var castObj = Expression.Convert(objParam, propertyInfo.DeclaringType!);
        var propertyAccess = Expression.Property(castObj, propertyInfo);
        var castResult = Expression.Convert(propertyAccess, typeof(object));
        var lambda = Expression.Lambda<Func<object, object?>>(castResult, objParam);
        return lambda.Compile();
    }

    public static bool IsPropertyEntityKey(PropertyInfo property)
    {
        return (property.GetCustomAttribute<KeyAttribute>() != null ||
               property.Name.Equals("id", StringComparison.OrdinalIgnoreCase)) &&
               property.PropertyType == typeof(int);
    }
    
    public static Type? GetCollectionElementType(Type collectionType)
    {
        if (collectionType.IsArray)
        {
            return collectionType.GetElementType();
        }

        var enumerableType = collectionType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType &&
                                i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        return enumerableType?.GetGenericArguments().FirstOrDefault();
    }
}
