using TodoAPI.Services;

namespace TodoAPI.EndpointFilters;

public static class ContextArgumentsHandler
{
    public static ITodoService GetService(EndpointFilterInvocationContext context, Type? serviceType = null)
    {
        foreach (var contextArgument in context.Arguments)
        {
            if (contextArgument is ITodoService service && serviceType is null)
            {
                return service;
            }
            
            if (contextArgument!.GetType() == serviceType)
            {
                return (ITodoService)contextArgument;
            }
        }

        throw new Exception("Context didn't provide service argument.");
    }
    
    public static int GetInt(EndpointFilterInvocationContext context)
    {
        foreach (var argument in context.Arguments)
        {
            if (argument?.GetType() == typeof(int)) 
                return (int)argument;
        }

        return -1;
    }
}