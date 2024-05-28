using System.Security.Claims;
using TodoAPI.Models;
using TodoAPI.Requests;
using TodoAPI.Services;

namespace TodoAPI.EndpointFilters;

public class UserClaimValidator : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        UserService service;
        
        try
        {
            service = (UserService)ContextArgumentsHandler.GetService(context, typeof(UserService));
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }

        var userClaim = GetUserClaimFromContext(context);
        var user = service.ClaimToUser(userClaim);

        if (user is null)
            return Results.NotFound("User with given Id was not found.");
        
        return await next(context);
    }

    private static ClaimsPrincipal? GetUserClaimFromContext(EndpointFilterInvocationContext context)
    {
        foreach (var argument in context.Arguments)
        {
            if (argument is ClaimsPrincipal userClaim)
                return userClaim;
        }

        return null;
    }
}