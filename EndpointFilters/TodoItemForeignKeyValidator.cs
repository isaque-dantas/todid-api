using TodoAPI.Models;
using TodoAPI.Services;

namespace TodoAPI.EndpointFilters;

public class TodoItemForeignKeyValidator : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        UserService userService;
        TodoListService todoListService;

        try
        {
            userService = (UserService)ContextArgumentsHandler.GetService(context, typeof(UserService));
            todoListService = (TodoListService)ContextArgumentsHandler.GetService(context, typeof(TodoListService));
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }

        var todoItemDto = GetTodoItemDtoFromContext(context);

        if (todoItemDto is null) return await next(context);

        if (!todoListService.EntryExists(todoItemDto.TodoListId))
            return Results.NotFound($"There isn't a TodoList with id '{todoItemDto.TodoListId}'");

        var userClaim = ContextArgumentsHandler.GetUserClaimFromContext(context);
        var user = userService.ClaimToUser(userClaim)!;
        
        if (!todoListService.UserHasEntry(todoItemDto.TodoListId, user.Id))
            return Results.Forbid();

        return await next(context);
    }

    private static TodoItemDto? GetTodoItemDtoFromContext(EndpointFilterInvocationContext context)
    {
        foreach (var contextArgument in context.Arguments)
        {
            if (contextArgument is TodoItemDto dto)
                return dto;
        }

        return null;
    }
}