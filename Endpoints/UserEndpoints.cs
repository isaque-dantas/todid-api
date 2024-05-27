using FluentValidation.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.EndpointFilters;
using TodoAPI.Requests;
using TodoAPI.Services;

namespace TodoAPI.Endpoints;

public static class UserEndpoints
{
    public static void Map(WebApplication app)
    {
        var users = app
            .MapGroup("/users")
            .AddEndpointFilter<FluentValidationEndpointFilter>();

        users.MapPost("/login", Login)
            .AddEndpointFilter<UserCredentialsValidator>();
    }

    public static IResult Login([FromBody] UserLoginRequest userLoginRequest, [FromServices] UserService service)
    {
        return Results.Ok();
    }
}