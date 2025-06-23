using BookTracker.Tools.ModelBinders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BookTracker.Tools.QueryComposer;
public class Filter
{
    public string PropertyName { get; set; }
    public string Value { get; set; }
    public FilterOperator Operator { get; set; }

    public Filter()
    {
        PropertyName = string.Empty;
        Value = string.Empty;
        Operator = FilterOperator.Equals;
    }

    public Filter(string propertyName, string value, FilterOperator filterOperator = FilterOperator.Equals)
    {
        PropertyName = propertyName;
        Value = value;
        Operator = filterOperator;
    }
}

class FilterFactory : IStringFactory<Filter>
{
    public static Filter Create(string value)
    {
        var parts = value.SplitWithEscape(';');
        if (parts.Length < 2)
            throw new ArgumentException("Invalid filter string format. Expected 'PropertyName;Value[;Operator]'");

        return new Filter(parts[0], parts[1], parts.Length > 2 ? OperatorfromString(parts[2]) : FilterOperator.Equals);
    }
    
    private static FilterOperator OperatorfromString(string operatorString)
    {
        return operatorString.ToLowerInvariant() switch
        {
            "neq" => FilterOperator.NotEquals,
            "gt" => FilterOperator.GreaterThan,
            "gte" => FilterOperator.GreaterThanOrEqual,
            "lt" => FilterOperator.LessThan,
            "lte" => FilterOperator.LessThanOrEqual,
            "ctn" => FilterOperator.Contains,
            _ => FilterOperator.Equals
        };
    }
}

public class FilterQueryArrayOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var parameters = context.ApiDescription.ParameterDescriptions;

        foreach (var parameter in parameters)
        {
            if (parameter.Type == typeof(Filter[]) && parameter.Source == BindingSource.Query)
            {
                var openApiParameter = operation.Parameters.FirstOrDefault(p => p.Name == parameter.Name && p.In == ParameterLocation.Query);
                if (openApiParameter == null) { continue; }
                openApiParameter.Schema = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema { Type = "string" }
                };
                openApiParameter.Description = $"Array of filters (as strings). Example: {parameter.Name}=(FIELD);(VALUE)(;OPERATOR)?";
            }
        }
    }
}