using System.Security.Claims;
using TodoAPI.Services;

namespace TodoAPI.EndpointFilters;

public class UserClaimValidator : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        UserService? service;

        try
        {
            service = ContextArgumentsHandler.GetArgument<UserService>(context);
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }

        var userClaim = ContextArgumentsHandler.GetArgument<ClaimsPrincipal>(context);
        var user = service!.ClaimToUser(userClaim);

        if (user is null)
            return Results.NotFound("User with given Id was not found.");

        return await next(context);
    }
}