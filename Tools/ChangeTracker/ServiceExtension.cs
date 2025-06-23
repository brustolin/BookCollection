using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BookTracker.Tools.ChangeTracker;

public static class ServiceCollectionExtensionsForChangeTracker
{
    public static IServiceCollection AddEntityChangeTracker(this IServiceCollection services)
    {
        services.AddSingleton<EntityChangeTracker>();
        return services;
    }
}

