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

        var (claimsUser, contextClaimsUserIndex) = GetClaimsUserFromContext(context);
        
        if (claimsUser is null)
            return Results.Problem("User data not found in request.");
        
        var userEmail = claimsUser.FindFirst(ClaimTypes.Email)?.Value;
        
        if (userEmail is null)
            return Results.NotFound($"User with e-mail '{userEmail}' was not found.");

        var user = service.GetByEmail(userEmail);

        if (user is null)
            return Results.NotFound($"User with e-mail '{userEmail}' was not found.");
        
        return await next(context);
    }

    private static (ClaimsPrincipal?, int) GetClaimsUserFromContext(EndpointFilterInvocationContext context)
    {
        var argumentIndex = 0;
        foreach (var argument in context.Arguments)
        {
            if (argument is ClaimsPrincipal userClaim)
                return (userClaim, argumentIndex);

            argumentIndex++;
        }

        return (null, 0);
    }
}