using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoAPI.Models;

public class TodoList
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }

    public int UserId { get; set; }

    [JsonIgnore] public User User { get; set; }

    public List<TodoItem>? TodoItems { get; set; }

    public TodoListDto ToTodoListDto()
    {
        var todoItemDtos = new List<TodoItemDto>();
        TodoItems?.ForEach(todoItem => { todoItemDtos.Add(todoItem.ToTodoItemDto()); });

        return new TodoListDto
        {
            Id = Id,
            Name = Name,
            Color = Color,
            UserId = UserId,
            TodoItems = todoItemDtos
        };
    }
}