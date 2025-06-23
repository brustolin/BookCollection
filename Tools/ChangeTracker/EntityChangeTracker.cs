using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

namespace BookTracker.Tools.ChangeTracker;

public class EntityChangeTracker
{
    private readonly ConcurrentDictionary<Type, TrackedEntityInfo> _trackedEntities = new();

    public bool IsEntityRegistered(Type type)
    {
        return _trackedEntities.ContainsKey(type);
    }

    public void RegisterEntity<SOURCE, TARGET>()
        where TARGET : EntityChange, new()
    {
        var entityType = typeof(SOURCE);
        if (_trackedEntities.ContainsKey(entityType))
        {
            return;
        }

        lock (_trackedEntities)
        {
            if (_trackedEntities.ContainsKey(entityType))
            {
                return;
            }
            var allProperties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var trackedProperties = new List<TrackedProperty>();
            PropertyInfo? idProperty = null;

            foreach (var property in allProperties)
            {
                if (GetTrackedInfo(property) is TrackedProperty trackedInfo)
                {
                    trackedProperties.Add(trackedInfo);
                }
                else if (ReflectionHelper.IsPropertyEntityKey(property))
                {
                    if (idProperty != null)
                    {
                        throw new InvalidOperationException($"Multiple ID properties found in {entityType.Name}. Only one ID property is allowed.");
                    }
                    idProperty = property;
                }
            }

            if (idProperty == null)
            {
                throw new InvalidOperationException($"No ID property found in {entityType.Name}. An ID property is required.");
            }

            var trackedEntityInfo = new TrackedEntityInfo(
                typeof(TARGET),
                idProperty,
                trackedProperties.ToArray()
            );
            _trackedEntities[entityType] = trackedEntityInfo;
        }
    }

    private static TrackedProperty? GetTrackedInfo(PropertyInfo property)
    {
        var attribute = property.GetCustomAttribute<BasePropertyMapAttribute>(true);
        if (attribute == null) return null;

        var attType = attribute.GetType();

        var attProperty = attType.GetProperty("PropertyInfo");
        if (attProperty == null) return null;

        PropertyInfo? descriptionProperty = null;
        if (attribute.DescriptionProperty is string descriptionPropertyName &&
            ReflectionHelper.GetCollectionElementType(property.PropertyType) is Type underlyingType)
        {
            descriptionProperty = underlyingType.GetProperty(descriptionPropertyName, BindingFlags.Public | BindingFlags.Instance);
        }

        var value = attProperty.GetValue(attribute);
        if (value != null && value.GetType().IsEnum && Enum.GetUnderlyingType(value.GetType()) == typeof(int))
        {
            return new TrackedProperty((int)value, property)
            {
                UnderlyingDescriptionProperty = descriptionProperty
            };
        }
        return null;
    }

    public IReadOnlyList<TARGET> DetectChanges<SOURCE, TARGET>(SOURCE oldObj, SOURCE newObj)
      where TARGET : EntityChange, new()
      where SOURCE : notnull
    {
        TrackedEntityInfo? trackInfo;
        lock (_trackedEntities)
        {
            if (!_trackedEntities.TryGetValue(typeof(SOURCE), out trackInfo))
            {
                this.RegisterEntity<SOURCE, TARGET>();
                trackInfo = _trackedEntities[typeof(SOURCE)];
            }
        }

        if (trackInfo == null)
        {
                return Array.Empty<TARGET>();
        }

        if (trackInfo.Target != typeof(TARGET))
        {
            throw new InvalidOperationException($"Storage class type {typeof(TARGET).Name} does not match with tracked class {typeof(SOURCE).Name} storage target.");
        }

        int sourceId;
        if (trackInfo.GetIdValue(oldObj) is int entityId)
        {
            sourceId = entityId;
        }
        else
        {
            throw new InvalidOperationException($"ID property {trackInfo.IdProperty.Name} must be of type int.");
        }

        var changes = new List<TARGET>();
        foreach (var prop in trackInfo.TrackedProperties)
        {
            var oldValue = prop.GetValue(oldObj);
            var newValue = prop.GetValue(newObj);

            if (oldValue is not string && newValue is not string &&
            oldValue is IEnumerable oldCollection &&
            newValue is IEnumerable newCollection)
            {
                changes.AddRange(GetCollectionDifference<TARGET>(oldCollection, newCollection, prop, sourceId));
            }
            else if (!Equals(oldValue, newValue))
            {
                changes.Add(new TARGET
                {
                    SourceId = sourceId,
                    PropertyId = prop.PropertyId,
                    OldValue = oldValue?.ToString() ?? string.Empty,
                    NewValue = newValue?.ToString() ?? string.Empty,
                    ChangeType = EntityChangeType.Updated,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        return changes;
    }
    
    private static IReadOnlyList<TARGET> GetCollectionDifference<TARGET>(IEnumerable oldCollection, IEnumerable newCollection, TrackedProperty propertyInfo, int sourceId)
        where TARGET : EntityChange, new()
    {
        var changes = new List<TARGET>();
        var oldSet = new HashSet<object>(oldCollection.Cast<object>());
        var newSet = new HashSet<object>(newCollection.Cast<object>());

        foreach (var item in oldSet.Except(newSet))
        {
            changes.Add(new TARGET
            {
                SourceId = sourceId,
                OldValue = propertyInfo?.GetUnderlyingDescriptionValue(item) ?? item.ToString() ?? string.Empty,
                NewValue = string.Empty,
                ChangeType = EntityChangeType.Deleted,
                Timestamp = DateTime.UtcNow
            });
        }

        foreach (var item in newSet.Except(oldSet))
        {
            changes.Add(new TARGET
            {
                SourceId = sourceId,
                OldValue = string.Empty,
                NewValue = propertyInfo?.GetUnderlyingDescriptionValue(item) ?? item.ToString() ?? string.Empty,
                ChangeType = EntityChangeType.Created,
                Timestamp = DateTime.UtcNow
            });
        }

        return changes;
    }
}