using System.Text.Json.Serialization;

namespace TodoAPI.Models;

public class TodoList
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    
    public int UserId { get; set; }
    
    public User User { get; set; }
    
    [JsonIgnore]
    public List<TodoItem>? TodoItems { get; set; }
}