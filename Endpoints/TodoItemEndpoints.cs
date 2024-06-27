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
            .AddEndpointFilter<RouteIdExistsValidator<TodoItem>>()
            .AddEndpointFilter<FluentValidationEndpointFilter>();

        items.MapGet("", GetAll);

        items.MapPost("", Create)
            .AddEndpointFilter<TodoItemForeignKeyValidator>();

        items.MapGet("/{id:int}", GetById)
            .AddEndpointFilter<UserHasRequestedEntryValidator>();

        items.MapPut("/{id:int}", Update)
            .AddEndpointFilter<TodoItemForeignKeyValidator>()
            .AddEndpointFilter<UserHasRequestedEntryValidator>();

        items.MapPatch("/{id:int}", ToggleIsComplete)
            .AddEndpointFilter<UserHasRequestedEntryValidator>();

        items.MapDelete("/{id:int}", DeleteById)
            .AddEndpointFilter<UserHasRequestedEntryValidator>();

        items.MapDelete("", DeleteAll);
    }

    public static IResult GetById([FromServices] TodoItemService todoItemService, int id,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        return Results.Ok(todoItemService.GetById(id));
    }

    public static IResult GetAll(HttpContext context, [FromServices] TodoItemService todoItemService,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        var user = userService.ClaimToUser(userClaim)!;
        return Results.Ok(todoItemService.GetAll(user.Id));
    }

    public static IResult Create([FromServices] TodoItemService todoItemService,
        [FromServices] TodoListService todoListService, [FromBody] TodoItemDto todoItemRequest,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        var todoList = todoListService.GetById(todoItemRequest.TodoListId);
        var todoItem = todoItemService.Create(todoItemRequest.ToTodoItem(), todoList);

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine($"*************** '{todoItem.Name}' ***************");
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine();

        return Results.Created($"/items/{todoItem.Id}", todoItem);
    }

    public static IResult Update([FromServices] TodoItemService todoItemService,
        [FromServices] TodoListService todoListService, [FromBody] TodoItemDto todoItemDto, int id,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        var todoList = todoListService.GetById(todoItemDto.TodoListId);
        todoItemService.Update(id, todoItemDto.ToTodoItem(), todoList);

        return Results.NoContent();
    }

    public static IResult ToggleIsComplete([FromServices] TodoItemService todoItemService, int id,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        todoItemService.ToggleIsComplete(id);
        return Results.NoContent();
    }

    public static IResult DeleteById([FromServices] TodoItemService todoItemService, int id,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        todoItemService.Delete(id);
        return Results.NoContent();
    }

    public static IResult DeleteAll([FromServices] TodoItemService todoItemService,
        [FromServices] UserService userService, ClaimsPrincipal userClaim)
    {
        var user = userService.ClaimToUser(userClaim)!;

        todoItemService.DeleteAll(user.Id);
        return Results.NoContent();
    }
}