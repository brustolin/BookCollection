using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BookTracker.Tools.ModelBinders;

public class FromStringModelBinderProvider<Factory, Type> : IModelBinderProvider
    where Factory : IStringFactory<Type>
    where Type : notnull
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if ((context.Metadata.ModelType == typeof(Type[]) || context.Metadata.ModelType == typeof(Type)) && context.BindingInfo.BindingSource == BindingSource.Query)
        {
            return new FromStringModelBinder<Factory, Type>();
        }
        return null;
    }
}