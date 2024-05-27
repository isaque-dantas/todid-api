using System.Security.Claims;
using FluentValidation.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.EndpointFilters;
using TodoAPI.Models;
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

        users.MapGet("", Get)
            .RequireAuthorization()
            .AddEndpointFilter<UserClaimValidator>();

        users.MapPost("", Register)
            .AddEndpointFilter<UserUniqueAttributesValidator>();

        users.MapPost("/login", Login)
            .AddEndpointFilter<UserCredentialsValidator>();
    }

    private static IResult Login([FromBody] UserLoginRequest userLoginRequest, [FromServices] UserService service)
    {
        var user = service.GetByEmail(userLoginRequest.Email)!;
        return Results.Ok(JwtBearerService.GenerateToken(user));
    }

    private static IResult Get([FromServices] UserService service, ClaimsPrincipal claimsUser)
    {
        return Results.Ok(user);
    }

    private static IResult Register([FromServices] UserService service, UserDto userDto)
    {
        var registeredUser = service.Register(userDto.ToUser());
        return Results.Created($"/{registeredUser.Id}", registeredUser);
    }
}