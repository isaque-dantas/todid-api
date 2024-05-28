using System.Security.Claims;
using FluentValidation.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.EndpointFilters;
using TodoAPI.Models;
using TodoAPI.Services;

namespace TodoAPI.Endpoints;

public static class TodoItemEndpoints
{
    public static void Map(WebApplication app)
    {
        var items = app
            .MapGroup("/items")
            .RequireAuthorization()
            .AddEndpointFilter<UserHasRequestedEntryValidator>()
            .AddEndpointFilter<RouteIdExistsValidator<TodoItem>>()
            .AddEndpointFilter<FluentValidationEndpointFilter>();

        items.MapGet("", GetAll);

        items.MapPost("", Create)
            .AddEndpointFilter<TodoItemForeignKeyValidator>();

        items.MapGet("/{id:int}", GetById);

        items.MapPut("/{id:int}", Update)
            .AddEndpointFilter<TodoItemForeignKeyValidator>();

        items.MapPatch("/{id:int}", ToggleIsComplete);

        items.MapDelete("/{id:int}", DeleteById);

        items.MapDelete("", DeleteAll);
    }

    private static IResult GetById([FromServices] TodoItemService todoItemService, int id,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        return Results.Ok(todoItemService.GetById(id));
    }

    private static IResult GetAll(HttpContext context, [FromServices] TodoItemService todoItemService,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        var user = userService.ClaimToUser(userClaim)!;
        return Results.Ok(todoItemService.GetAll(user.Id));
    }

    private static IResult Create([FromServices] TodoItemService todoItemService,
        [FromServices] TodoListService todoListService, [FromBody] TodoItemDto todoItemRequest,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        var todoList = todoListService.GetById(todoItemRequest.TodoListId);
        var todoItem = todoItemService.Create(todoItemRequest.ToTodoItem(), todoList);

        return Results.Created($"/items/{todoItem.Id}", todoItem);
    }

    private static IResult Update([FromServices] TodoItemService todoItemService,
        [FromServices] TodoListService todoListService, [FromBody] TodoItemDto todoItemDto, int id,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        var todoList = todoListService.GetById(todoItemDto.TodoListId);
        todoItemService.Update(id, todoItemDto.ToTodoItem(), todoList);

        return Results.NoContent();
    }

    private static IResult ToggleIsComplete([FromServices] TodoItemService todoItemService, int id,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        todoItemService.ToggleIsComplete(id);
        return Results.NoContent();
    }

    private static IResult DeleteById([FromServices] TodoItemService todoItemService, int id,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        todoItemService.Delete(id);
        return Results.NoContent();
    }

    private static IResult DeleteAll([FromServices] TodoItemService todoItemService,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        var user = userService.ClaimToUser(userClaim)!;

        todoItemService.DeleteAll(user.Id);
        return Results.NoContent();
    }
}