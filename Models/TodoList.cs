using System.Text.Json.Serialization;

namespace TodoAPI.Models;

public class TodoList
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    
    [JsonIgnore]
    public List<TodoItem>? TodoItems { get; set; } = new List<TodoItem>();
}