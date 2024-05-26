using TodoAPI.Models;
using TodoAPI.Services;

namespace TodoAPI.EndpointFilters;

public class TodoItemForeignKeyValidator : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        ITodoService service;

        try
        {
            service = ContextArgumentsHandler.GetService(context, typeof(TodoListService));
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }

        var todoItemDto = GetTodoItemDtoFromContext(context);
        
        if (todoItemDto is null) return await next(context);
        
        if (!service.EntryExists(todoItemDto.TodoListId))
            return Results.NotFound($"There isn't a TodoList with id '{todoItemDto.TodoListId}'");

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