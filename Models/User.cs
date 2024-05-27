using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Models;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Username), IsUnique = true)]
public class User
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(32)]
    public string Username { get; set; }
    
    [Required]
    [MaxLength(64)]
    public string Name { get; set; }
    
    [Required]
    [MaxLength(64)]
    public string Email { get; set; }
    
    [Required]
    [MaxLength(24)]
    public string Password { get; set; }
    
    [Required]
    public string Role { get; set; }
    
    [JsonIgnore]
    public List<TodoList>? TodoLists { get; set; }
}