using TodoAPI.Services;

namespace TodoAPI.EndpointFilters;

public class RouteIdExistsValidator<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var entryId = ContextArgumentsHandler.GetArgument<int?>(context);
        if (entryId is null) return await next.Invoke(context);

        ITodoService? service;
        var serviceType = ContextArgumentsHandler.GetRequestedEntryServiceType(context);

        try
        {
            service = (ITodoService?)ContextArgumentsHandler.GetArgument(context, serviceType);
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }

        if (!service!.EntryExists((int)entryId))
            return Results.NotFound($"There isn't any {GetEntryName()} with id '{entryId}'.");

        return await next.Invoke(context);
    }

    private static string? GetEntryName()
    {
        return typeof(T).FullName?.Split(".").Last();
    }
}