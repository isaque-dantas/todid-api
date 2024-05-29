using System.Net;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using TodoAPI.Models;
using TodoAPI.Services;

namespace TodoAPI.EndpointFilters;

public class UserUniqueAttributesValidator : IEndpointFilter
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

        var userDto = ContextArgumentsHandler.GetArgument<UserDto>(context);
        if (userDto is null)
            return Results.Problem("User was not found in the request.");

        var userClaim = ContextArgumentsHandler.GetArgument<ClaimsPrincipal>(context);
        var loggedUser = service!.ClaimToUser(userClaim);
        
        var validationProblems = 
            GetValidationProblems(UserDto.GetUniqueAttributes(service, userDto), loggedUser);
        
        if (!validationProblems.IsNullOrEmpty())
            return Results.ValidationProblem(validationProblems, statusCode: 400);

        return await next(context);
    }

    private static Dictionary<string, string[]> GetValidationProblems(List<UniqueAttribute<string>> uniqueAttributes, User? loggedUser)
    {
        var validationProblems = new Dictionary<string, string[]>();
        
        foreach (var attribute in uniqueAttributes)
        {
            var searchedUser = attribute.ServiceSearcherMethod(attribute.Value);
            if (searchedUser is not null && searchedUser.Id != loggedUser?.Id)
            {
                validationProblems.Add(
                    attribute.Name, [$"'{attribute.Name}' was already registered with value '{attribute.Value}'"]
                );
            }
        }

        return validationProblems;
    }
}