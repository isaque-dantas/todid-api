using Microsoft.EntityFrameworkCore;
using TodoList = TodoAPI.Models.TodoList;
using TodoItem = TodoAPI.Models.TodoItem;

namespace TodoAPI.Services;

public class TodoItemService(TodoContext context) : ITodoService
{
    public bool EntryExists(int id)
    {
        var todoItem = context.TodoItems.Find(id);
        return todoItem is not null;
    }

    public TodoItem GetById(int id)
    {
        return context.TodoItems.Find(id)!;
    }

    public IEnumerable<TodoItem> GetAll(int userId)
    {
        return context.TodoItems
            .AsNoTracking()
            .Where(item => item.TodoList.UserId == userId)
            .ToList();
    }

    public TodoItem Create(TodoItem newTodoItem, TodoList todoList)
    {
        newTodoItem.TodoList = todoList;

        context.TodoItems.Add(newTodoItem);
        context.SaveChanges();

        return newTodoItem;
    }

    public void Update(int id, TodoItem inputTodoItem, TodoList todoList)
    {
        var todo = GetById(id);

        todo.TodoList = todoList;
        todo.TodoListId = todoList.Id;
        todo.IsComplete = inputTodoItem.IsComplete;
        todo.Name = inputTodoItem.Name;

        context.SaveChanges();
    }

    public void Delete(int id)
    {
        var todo = GetById(id);

        context.TodoItems.Remove(todo);
        context.SaveChanges();
    }

    public void DeleteAll(int userId)
    {
        context.TodoItems.RemoveRange(context.TodoItems.Where(item => item.TodoList.UserId == userId));
        context.SaveChanges();
    }

    public void ToggleIsComplete(int id)
    {
        var todo = GetById(id);

        todo.IsComplete = !todo.IsComplete;
        context.SaveChanges();
    }
}