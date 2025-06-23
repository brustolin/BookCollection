using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BookTracker.Tools.ModelBinders;

public class FromStringModelBinder<Factory, Type> : IModelBinder
    where Factory : IStringFactory<Type>
    where Type : notnull
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var values = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (values == ValueProviderResult.None)
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        object? result = bindingContext.ModelType.IsArray
            ? values.Select(v => Factory.Create(v)).ToArray()
            : Factory.Create(values.FirstOrDefault() ?? string.Empty);

        bindingContext.Result = ModelBindingResult.Success(result);
        return Task.CompletedTask;
    }
}
