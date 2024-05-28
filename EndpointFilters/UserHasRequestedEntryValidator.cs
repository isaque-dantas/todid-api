using TodoAPI.Services;

namespace TodoAPI.EndpointFilters;

public class UserHasRequestedEntryValidator : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var requestPathToServiceType = new Dictionary<string, Type>
        {
            { "items", typeof(TodoItemService) },
            { "lists", typeof(TodoListService) },
        };

        var requestPath = (string)context.HttpContext.Request.Path;
        var entryServiceType = requestPathToServiceType[requestPath.Split("/")[1]];

        try
        {
            var userService = ContextArgumentsHandler.GetService(context, typeof(UserService));
            var entryService = ContextArgumentsHandler.GetService(context, entryServiceType);
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }

        

        return await next(context);
    }
}