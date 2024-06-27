using TodoAPI.Requests;
using TodoAPI.Services;

namespace TodoAPI.EndpointFilters;

public class UserCredentialsValidator : IEndpointFilter
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

        var request = ContextArgumentsHandler.GetArgument<UserLoginRequest>(context);
        if (request is null)
            return Results.Problem("User login data not found.");

        if (service!.GetByEmail(request.Email) is null)
            return Results.NotFound($"There isn't any user with email '{request.Email}'");

        if (!service.AreEmailAndPasswordValid(request.Email, request.Password))
            return Results.Unauthorized();

        return await next(context);
    }
}