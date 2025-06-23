using System;
namespace BookTracker.Tools.ChangeTracker;

public class BasePropertyMapAttribute : Attribute
{
    public string? DescriptionProperty { get; set; }
    
    public BasePropertyMapAttribute(string? descriptionProperty = null)
    {
        DescriptionProperty = descriptionProperty;
    }
}

public class PropertyMapAttribute<T> : BasePropertyMapAttribute where T : Enum
{
    public T PropertyInfo { get; set; }
    
    public PropertyMapAttribute(T propertyInfo, string? descriptionProperty = null) : base(descriptionProperty)
    {
        PropertyInfo = propertyInfo;
    }
}