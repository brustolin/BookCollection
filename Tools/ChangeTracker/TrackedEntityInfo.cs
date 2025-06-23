using System.Reflection;

namespace BookTracker.Tools.ChangeTracker;

class TrackedEntityInfo
{
    public Type Target { get; set; }
    public PropertyInfo IdProperty { get; set; }
    public TrackedProperty[] TrackedProperties { get; set; }
    private readonly Func<object, object?> _idGetter;

    public TrackedEntityInfo(Type targetType, PropertyInfo idProperty, TrackedProperty[] trackedProperties)
    {
        Target = targetType;
        IdProperty = idProperty;
        TrackedProperties = trackedProperties;

        _idGetter = ReflectionHelper.CreateGetter(idProperty);
    }
    
    public object? GetIdValue(object obj)
    {
        return _idGetter(obj);
    }
}
