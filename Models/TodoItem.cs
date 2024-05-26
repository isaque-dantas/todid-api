using System.ComponentModel.DataAnnotations;

namespace TodoAPI.Models;

public class TodoItem
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    [Required]
    public bool IsComplete { get; set; }
    
    public int TodoListId { get; set; } 
    public TodoList TodoList { get; set; } 
}