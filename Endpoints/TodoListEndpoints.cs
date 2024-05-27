using FluentValidation.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.EndpointFilters;
using TodoAPI.Models;
using TodoAPI.Requests;
using TodoAPI.Services;

namespace TodoAPI;

public static class TodoListEndpoints
{
    public static void Map(WebApplication app)
    {
        var lists = app
            .MapGroup("/lists")
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

    private static IResult GetById([FromServices] TodoListService service, int id)
    {
        return Results.Ok(service.GetById(id));
    }

    private static IResult GetAll([FromServices] TodoListService service)
    {
        return Results.Ok(service.GetAll());
    }

    private static IResult GetItemsFromList([FromServices] TodoListService service, int id)
    {
        return Results.Ok(service.GetItems(id));
    }

    private static IResult Create([FromServices] TodoListService service, [FromBody] TodoListDto todoListDto)
    {
        var todoList = service.Create(todoListDto.ToTodoList());
        return Results.Created($"/lists/{todoList.Id}", todoList);
    }

    private static IResult Update([FromServices] TodoListService service, [FromBody] UpdateTodoListRequest todoListRequest,
        int id)
    {
        service.Update(id, todoListRequest.ToTodoList());
        return Results.NoContent();
    }

    private static IResult DeleteById([FromServices] TodoListService service, int id)
    {
        service.DeleteById(id);
        return Results.NoContent();
    }

    private static IResult DeleteItemsById([FromServices] TodoListService service, int id)
    {
        service.DeleteItemsById(id);
        return Results.NoContent();
    }

    private static IResult DeleteAll([FromServices] TodoListService service)
    {
        service.DeleteAll();
        return Results.NoContent();
    }
}