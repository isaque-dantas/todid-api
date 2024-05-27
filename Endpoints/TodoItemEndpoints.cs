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
            .AddEndpointFilter<RouteIdExistsValidator<TodoItem>>()
            .AddEndpointFilter<FluentValidationEndpointFilter>();

        items.MapGet("", GetAll)
            .RequireAuthorization("admin_greetings");

        items.MapPost("", Create)
            .AddEndpointFilter<TodoItemForeignKeyValidator>();

        items.MapGet("/{id:int}", GetById);

        items.MapPut("/{id:int}", Update)
            .AddEndpointFilter<TodoItemForeignKeyValidator>();

        items.MapPatch("/{id:int}", ToggleIsComplete);

        items.MapDelete("/{id:int}", DeleteById);

        items.MapDelete("", DeleteAll);
    }

    private static IResult GetById([FromServices] TodoItemService service, int id)
    {
        return Results.Ok(service.GetById(id));
    }

    private static IResult GetAll(HttpContext context, [FromServices] TodoItemService service)
    {
        Console.WriteLine(context.User.FindFirst(ClaimTypes.Sid)?.Value);
        return Results.Ok(service.GetAll());
    }

    private static IResult Create([FromServices] TodoItemService todoItemService,
        [FromServices] TodoListService todoListService, [FromBody] TodoItemDto todoItemRequest)
    {
        var todoList = todoListService.GetById(todoItemRequest.TodoListId);
        var todoItem = todoItemService.Create(todoItemRequest.ToTodoItem(), todoList);

        return Results.Created($"/items/{todoItem.Id}", todoItem);
    }

    private static IResult Update([FromServices] TodoItemService todoItemService,
        [FromServices] TodoListService todoListService, [FromBody] TodoItemDto todoItemDto, int id)
    {
        var todoList = todoListService.GetById(todoItemDto.TodoListId);
        todoItemService.Update(id, todoItemDto.ToTodoItem(), todoList);

        return Results.NoContent();
    }

    private static IResult ToggleIsComplete([FromServices] TodoItemService service, int id)
    {
        service.ToggleIsComplete(id);
        return Results.NoContent();
    }

    private static IResult DeleteById([FromServices] TodoItemService service, int id)
    {
        service.Delete(id);
        return Results.NoContent();
    }

    private static IResult DeleteAll([FromServices] TodoItemService service)
    {
        service.DeleteAll();
        return Results.NoContent();
    }
}