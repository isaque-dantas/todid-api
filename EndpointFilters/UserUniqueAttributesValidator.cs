using System.Net;
using Microsoft.IdentityModel.Tokens;
using TodoAPI.Models;
using TodoAPI.Services;

namespace TodoAPI.EndpointFilters;

public class UserUniqueAttributesValidator : IEndpointFilter
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

        var userDto = GetUserDtoFromContext(context);
        if (userDto is null)
            return Results.Problem("User was not found in the request.");

        var uniqueAttributes = new List<UniqueAttribute<string>>
        {
            new() { Name = "Email", ServiceSearcherMethod = service.GetByEmail!, Value = userDto.Email },
            new() { Name = "Username", ServiceSearcherMethod = service.GetByUsername!, Value = userDto.Username }
        };

        var userClaim = ContextArgumentsHandler.GetUserClaimFromContext(context);
        var loggedUser = service.ClaimToUser(userClaim);
        
        var validationProblems = GetValidationProblems(uniqueAttributes, loggedUser);
        
        if (!validationProblems.IsNullOrEmpty())
            return Results.ValidationProblem(validationProblems, statusCode: 400);

        return await next(context);
    }

    private static UserDto? GetUserDtoFromContext(EndpointFilterInvocationContext context)
    {
        foreach (var contextArgument in context.Arguments)
        {
            if (contextArgument is UserDto dto)
                return dto;
        }

        return null;
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