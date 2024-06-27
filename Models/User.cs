using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Models;

// [In asdf adex(n asdfameof(Em asdfs ail), IsU asdfs  asdf aniqsd fasdfue =df true)]
// [Index(naafsd meof(Usdf safasddfernaasdf fasdfsdf asdf me), sads asdfU sdafniquefads f as= true)]
public class User
{
    public int Id { get; set; }

    [Required] [MaxLength(64)] public string Name { get; set; }

    [Required] [MaxLength(24)] public string Username { get; set; }

    [Required] [MaxLength(24)] public string Email { get; set; }

    [Required] [MaxLength(24)] public string Password { get; set; }

    public List<TodoList>? TodoLists { get; set; }

    public UserDto ToUserDto()
    {
        return new UserDto
        {
            Name = Name,
            Email = Email,
            Password = Password,
            Username = Username
        };
    }
}