using System.Security.Claims;
using FluentValidation.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Authentication;
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

        users.MapPut("", Update)
            .RequireAuthorization()
            .AddEndpointFilter<UserClaimValidator>()
            .AddEndpointFilter<UserUniqueAttributesValidator>();

        users.MapDelete("", Delete)
            .RequireAuthorization()
            .AddEndpointFilter<UserClaimValidator>();
    }

    public static IResult Login([FromBody] UserLoginRequest userLoginRequest, [FromServices] UserService service)
    {
        var user = service.GetByEmail(userLoginRequest.Email)!;
        return Results.Ok(JwtBearerService.GenerateToken(user));
    }

    public static IResult Get([FromServices] UserService service, ClaimsPrincipal userClaim)
    {
        return Results.Ok(service.ClaimToUser(userClaim));
    }

    public static IResult Register([FromServices] UserService service, UserDto userDto)
    {
        var registeredUser = service.Register(userDto.ToUser());

        var registeredUserDto = registeredUser.ToUserDto();
        return Results.Created($"/{registeredUser.Id}", registeredUserDto);
    }

    public static IResult Update([FromServices] UserService service, ClaimsPrincipal userClaim, UserDto userDto)
    {
        var user = service.ClaimToUser(userClaim)!;
        service.Update(userDto.ToUser(), user.Id);

        return Results.NoContent();
    }

    public static IResult Delete([FromServices] UserService service, ClaimsPrincipal userClaim)
    {
        var user = service.ClaimToUser(userClaim)!;
        service.DeleteById(user.Id);

        return Results.NoContent();
    }
}