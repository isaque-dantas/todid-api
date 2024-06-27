using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoAPI.Models;

public class TodoItem
{
    public int Id { get; set; }

    [Required] [MaxLength(100)] public string Name { get; set; }

    [Required] public bool IsComplete { get; set; }

    public int TodoListId { get; set; }
    [JsonIgnore] public TodoList TodoList { get; set; }

    public TodoItemDto ToTodoItemDto()
    {
        return new TodoItemDto
        {
            Id = Id,
            Name = Name,
            IsComplete = IsComplete,
            TodoListId = TodoListId
        };
    }
}