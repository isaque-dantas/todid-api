using System.Security.Claims;
using TodoAPI.Models;
using TodoAPI.Services;

namespace TodoAPI.EndpointFilters;

public class TodoItemForeignKeyValidator : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        UserService? userService;
        TodoListService? todoListService;

        try
        {
            userService = ContextArgumentsHandler.GetArgument<UserService>(context);
            todoListService = ContextArgumentsHandler.GetArgument<TodoListService>(context);
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }

        var todoItemDto = ContextArgumentsHandler.GetArgument<TodoItemDto>(context);

        if (todoItemDto is null) return await next(context);

        if (!todoListService!.EntryExists(todoItemDto.TodoListId))
            return Results.NotFound($"There isn't a TodoList with id '{todoItemDto.TodoListId}'");

        var userClaim = ContextArgumentsHandler.GetArgument<ClaimsPrincipal>(context);
        var user = userService!.ClaimToUser(userClaim)!;

        if (!userService.UserHasEntry(user.Id, todoItemDto.TodoListId, typeof(TodoList)))
            return Results.Forbid();

        return await next(context);
    }
}