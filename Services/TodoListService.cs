using Microsoft.EntityFrameworkCore;
using TodoList = TodoAPI.Models.TodoList;
using TodoItem = TodoAPI.Models.TodoItem;


namespace TodoAPI.Services;

public class TodoListService(TodoContext context) : ITodoService
{
    public bool EntryExists(int id)
    {
        var todoList = context.TodoLists.Find(id);
        return todoList is not null;
    }
    
    public IEnumerable<TodoList> GetAll(int userId)
    {
        return context.TodoLists
            .AsNoTracking()
            .Where(list => list.UserId == userId)
            .ToList();
    }

    public TodoList GetById(int id)
    {
        return context.TodoLists.Find(id)!;
    }

    public List<TodoItem> GetItems(int id)
    {
        var items =
            context.TodoItems.Where(item => item.TodoListId == id).ToList();
        return items;
    }

    public TodoList Create(TodoList newTodoList)
    {
        newTodoList.User = context.Users.Find(newTodoList.UserId)!;
        context.TodoLists.Add(newTodoList);
        context.SaveChanges();

        return newTodoList;
    }

    public void Update(int id, TodoList inputTodoList)
    {
        var todoList = GetById(id);
        todoList.Name = inputTodoList.Name;
        todoList.Color = inputTodoList.Color;
        if (inputTodoList.TodoItems!.Count > 0)
        {
            context.RemoveRange(GetItems(todoList.Id));

            foreach (var todoItemDto in inputTodoList.TodoItems)
            {
                var todoItem = new TodoItem
                {
                    Name = todoItemDto.Name,
                    IsComplete = todoItemDto.IsComplete,
                    TodoListId = todoItemDto.TodoListId,
                    TodoList = todoList
                };

                context.TodoItems.Add(todoItem);
            }
        }

        context.SaveChanges();
    }

    public void DeleteById(int id)
    {
        context.TodoLists.Remove(GetById(id));
        context.SaveChanges();
    }

    public void DeleteItemsById(int id)
    {
        context.TodoItems.RemoveRange(GetItems(id));
        context.SaveChanges();
    }

    public void DeleteAll(int userId)
    {
        context.TodoLists.RemoveRange(
            context.TodoLists.Where(list => list.UserId == userId)
        );
        context.SaveChanges();
    }
}