using TodoAPI.Models;
using TodoAPI.Services;

namespace TodoAPI.EndpointFilters;

public static class ContextArgumentsHandler
{
    public static T? GetArgument<T>(EndpointFilterInvocationContext context)
    {
        return (T?)context.Arguments.SingleOrDefault(argument => argument is T);
    }

    public static object? GetArgument(EndpointFilterInvocationContext context, Type argumentType)
    {
        return context.Arguments.SingleOrDefault(argument => argument?.GetType() == argumentType);
    }

    public static Type GetRequestedEntryServiceType(EndpointFilterInvocationContext context)
    {
        var requestPathToServiceType = new Dictionary<string, Type>
        {
            { "items", typeof(TodoItemService) },
            { "lists", typeof(TodoListService) },
            { "users", typeof(UserService) }
        };

        var requestedEntryName = GetRequestedEntryName(context);
        return requestPathToServiceType[requestedEntryName];
    }

    public static Type GetRequestedEntryType(EndpointFilterInvocationContext context)
    {
        var requestPathToServiceType = new Dictionary<string, Type>
        {
            { "items", typeof(TodoItem) },
            { "lists", typeof(TodoList) },
            { "users", typeof(User) }
        };

        var requestedEntryName = GetRequestedEntryName(context);
        return requestPathToServiceType[requestedEntryName];
    }

    public static string GetRequestedEntryName(EndpointFilterInvocationContext context)
    {
        var requestPath = (string)context.HttpContext.Request.Path;
        return requestPath.Split("/")[1];
    }
}