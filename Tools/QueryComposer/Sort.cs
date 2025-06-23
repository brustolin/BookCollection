using BookTracker.Tools.ModelBinders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BookTracker.Tools.QueryComposer;

public class Sort
{
    public string SortBy { get; set; }
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

    public Sort()
    {
        SortBy = string.Empty;
        SortDirection = SortDirection.Ascending;
    }

    public Sort(string sortBy, SortDirection sortDirection = SortDirection.Ascending)
    {
        SortBy = sortBy;
        SortDirection = sortDirection;
    }
}

public class SortFactory : IStringFactory<Sort>
{
    public static Sort Create(string value)
    {
        var parts = value.SplitWithEscape(';');
        if (parts.Length < 1)
            throw new ArgumentException("Invalid sort string format. Expected '(FIELD)(;DIRECTION)?'");

        var direction = parts.Length > 1 && parts[1] == "desc"
            ? SortDirection.Descending
            : SortDirection.Ascending;

        return new Sort(parts[0], direction);
    }
}

public class SortQueryArrayOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var parameters = context.ApiDescription.ParameterDescriptions;
        foreach (var parameter in parameters)
        {
            if (parameter.Type == typeof(Sort[]) && parameter.Source == BindingSource.Query)
            {
                var openApiParameter = operation.Parameters.FirstOrDefault(p => p.Name == parameter.Name && p.In == ParameterLocation.Query);
                if (openApiParameter == null) { continue; }
                openApiParameter.Schema = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema { Type = "string" }
                };
                openApiParameter.Description = $"Array of sort queries (as strings). Example: {parameter.Name}=(FIELD);(DIRECTION)?";
            }
        }
    }
}
