using System.Security.Claims;
using TodoAPI.Services;

namespace TodoAPI.EndpointFilters;

public class UserHasRequestedEntryValidator : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        UserService? userService;

        try
        {
            userService = ContextArgumentsHandler.GetArgument<UserService>(context);
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }

        var userClaim = ContextArgumentsHandler.GetArgument<ClaimsPrincipal>(context);
        if (userClaim is null)
            return Results.Problem("Context didn't provide user information.");

        var user = userService!.ClaimToUser(userClaim)!;
        var entryId = ContextArgumentsHandler.GetArgument<int>(context);

        var entryType = ContextArgumentsHandler.GetRequestedEntryType(context);

        if (!userService.UserHasEntry(user.Id, entryId, entryType))
            return Results.Forbid();

        return await next(context);
    }
}