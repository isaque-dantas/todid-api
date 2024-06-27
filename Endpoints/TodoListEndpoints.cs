using System.Security.Claims;
using FluentValidation.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.EndpointFilters;
using TodoAPI.Models;
using TodoAPI.Requests;
using TodoAPI.Services;

namespace TodoAPI.Endpoints;

public static class TodoListEndpoints
{
    public static void Map(WebApplication app)
    {
        var lists = app
            .MapGroup("/lists")
            .RequireAuthorization()
            .AddEndpointFilter<RouteIdExistsValidator<TodoList>>()
            .AddEndpointFilter<FluentValidationEndpointFilter>();

        lists.MapGet("/{id:int}", GetById);

        lists.MapGet("/{id:int}/items", GetItemsFromList);

        lists.MapGet("", GetAll);

        lists.MapPost("", Create);

        lists.MapPut("/{id:int}", Update);

        lists.MapDelete("/{id:int}", DeleteById);

        lists.MapDelete("/{id:int}/items", DeleteItemsById);

        lists.MapDelete("", DeleteAll);
    }

    public static IResult GetById([FromServices] TodoListService todoListService, int id,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        return Results.Ok(todoListService.GetById(id));
    }

    public static IResult GetAll([FromServices] TodoListService todoListService,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        var user = userService.ClaimToUser(userClaim)!;
        return Results.Ok(todoListService.GetAll(user.Id));
    }

    public static IResult GetItemsFromList([FromServices] TodoListService todoListService, int id,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        return Results.Ok(todoListService.GetItems(id));
    }

    public static IResult Create([FromServices] TodoListService todoListService,
        [FromServices] UserService userService, [FromBody] TodoListDto todoListDto, ClaimsPrincipal userClaim)
    {
        var user = userService.ClaimToUser(userClaim)!;
        todoListDto.UserId = user.Id;

        var todoList = todoListService.Create(todoListDto.ToTodoList());
        return Results.Created($"/lists/{todoList.Id}", todoList);
    }

    public static IResult Update([FromServices] TodoListService todoListService,
        [FromBody] UpdateTodoListRequest todoListRequest, int id,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        var user = userService.ClaimToUser(userClaim)!;
        todoListRequest.UserId = user.Id;

        todoListService.Update(id, todoListRequest.ToTodoList());
        return Results.NoContent();
    }

    public static IResult DeleteById([FromServices] TodoListService todoListService, int id,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        todoListService.DeleteById(id);
        return Results.NoContent();
    }

    public static IResult DeleteItemsById([FromServices] TodoListService todoListService, int id,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        todoListService.DeleteItemsById(id);
        return Results.NoContent();
    }

    public static IResult DeleteAll([FromServices] TodoListService todoListService,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        var user = userService.ClaimToUser(userClaim)!;

        todoListService.DeleteAll(user.Id);
        return Results.NoContent();
    }
}