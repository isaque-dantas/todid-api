using TodoAPI.Services;

namespace TodoAPI.EndpointFilters;

public class RouteIdValidator<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var entryId = ContextArgumentsHandler.GetInt(context);
        if (entryId == -1) return await next.Invoke(context);

        ITodoService service;

        try
        {
            service = ContextArgumentsHandler.GetService(context);
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }

        if (!service.EntryExists(entryId))
        {
            return Results.NotFound($"There isn't any {GetEntryName()} with id '{entryId}'.");
        }

        return await next.Invoke(context);
    }

    private static string? GetEntryName()
    {
        return typeof(T).FullName?.Split(".").Last();
    }
}