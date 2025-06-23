using System.Reflection;

namespace BookTracker.Tools.ChangeTracker;

class TrackedProperty
{
    public int PropertyId { get; set; }
    public PropertyInfo PropertyInfo { get; set; }
    public PropertyInfo? UnderlyingDescriptionProperty { get; set; }
    private readonly Func<object, object?> _propertyGetter;
    private readonly Func<object, object?>? _underlyingDescriptionGetter;

    public TrackedProperty(int propertyId, PropertyInfo propertyInfo, PropertyInfo? underlyingDescriptionProperty = null)
    {
        PropertyId = propertyId;
        PropertyInfo = propertyInfo;
        UnderlyingDescriptionProperty = underlyingDescriptionProperty;
        _propertyGetter = ReflectionHelper.CreateGetter(propertyInfo);
        if (underlyingDescriptionProperty != null)
        {
            _underlyingDescriptionGetter = ReflectionHelper.CreateGetter(underlyingDescriptionProperty);
        }
    }

    public object? GetValue(object obj)
    {
        return _propertyGetter(obj);
    }

    public string? GetUnderlyingDescriptionValue(object obj)
    {
        return _underlyingDescriptionGetter?.Invoke(obj)?.ToString();
    }    
}
