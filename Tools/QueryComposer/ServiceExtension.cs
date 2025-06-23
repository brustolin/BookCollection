using BookTracker.Tools.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BookTracker.Tools.QueryComposer;

public static class ServiceCollectionExtensionsForQueryComposer
{
    public static IServiceCollection AddEntityQueryComposer(this IServiceCollection services)
    {
        services.AddSingleton<EntityQueryComposer>();
        services.Configure<MvcOptions>(options =>
        {
            options.ModelBinderProviders.Insert(0, new FromStringModelBinderProvider<FilterFactory, Filter>());
            options.ModelBinderProviders.Insert(0, new FromStringModelBinderProvider<SortFactory, Sort>());
        });
    
        return services;
    }

    // This could be moved to a separated nuget package in order to be used in projects
    // that use the Swashbuckle.AspNetCore.SwaggerGen.
    public static void AddEntityQueryComposerSwagger(this IServiceCollection services)
    {
        services.Configure<SwaggerGenOptions>(options =>
        {
            options.OperationFilter<FilterQueryArrayOperationFilter>();
            options.OperationFilter<SortQueryArrayOperationFilter>();
        });
    }
}

