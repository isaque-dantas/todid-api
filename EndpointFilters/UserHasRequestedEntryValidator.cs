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

        UserService userService;
        ITodoService entryService;

        try
        {
            userService = (UserService)ContextArgumentsHandler.GetService(context, typeof(UserService));
            entryService = ContextArgumentsHandler.GetService(context, entryServiceType);
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }

        var userClaim = ContextArgumentsHandler.GetUserClaimFromContext(context);
        if (userClaim is null)
            return Results.Problem("Context didn't provide user information.");

        var user = userService.ClaimToUser(userClaim)!;
        var entryId = ContextArgumentsHandler.GetInt(context);

        if (!entryService.UserHasEntry(entryId, user.Id))
            return Results.NotFound(
                $"{entryServiceType.FullName} didn't localize entry with id '{entryId}' that belongs to user '{user.Username}'."
            );

        return await next(context);
    }
}