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
            .AddEndpointFilter<RouteIdExistsValidator<TodoList>>()
            .AddEndpointFilter<FluentValidationEndpointFilter>();

        lists.MapGet("/{id:int}", GetById);

        lists.MapGet("/{id:int}/items", GetItemsFromList);

        lists.MapGet("", GetAll)
            .RequireAuthorization();

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

    private static IResult GetAll([FromServices] TodoListService todoListService,
        [FromServices] UserService userService, ClaimsPrincipal claimsUser)
    {
        var userEmail = claimsUser.FindFirst(ClaimTypes.Email)?.Value;
        if (userEmail is null)
            return Results.NotFound("User was not found!");
        
        var user = userService.GetByEmail(userEmail);

        if (user is null)
            return Results.NotFound("User was not found!");
        
        return Results.Ok(todoListService.GetAll(user.Id));
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

    private static IResult Update([FromServices] TodoListService service,
        [FromBody] UpdateTodoListRequest todoListRequest,
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