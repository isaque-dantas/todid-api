using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;

namespace TodoAPI.Services;

public class UserService(TodoContext context) : ITodoService
{
    public bool EntryExists(int id)
    {
        var user = context.Users.Find(id);
        return user is not null;
    }

    public bool UserHasEntry(int id, int entryId, Type entryType)
    {
        int todoListId;

        if (entryType == typeof(TodoItem))
            todoListId = context.TodoItems.Find(entryId)!.TodoListId;
        else if (entryType == typeof(TodoList))
            todoListId = entryId;
        else
            return false;

        return context.TodoLists.Find(todoListId)!.UserId == id;
    }

    public bool AreEmailAndPasswordValid(string email, string password)
    {
        var user = context.Users.AsNoTracking().SingleOrDefault(u => u.Email == email);

        if (user is null)
            return false;

        return user.Password == password;
    }

    public User? ClaimToUser(ClaimsPrincipal? userClaim)
    {
        var userId = userClaim?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userId, out _) ? GetById(int.Parse(userId)) : null;
    }

    public User? GetById(int id)
    {
        var user = context.Users.SingleOrDefault(u => u.Id == id);
        return user;
    }

    public User? GetByUsername(string username)
    {
        var user = context.Users.SingleOrDefault(u => u.Username == username);
        return user;
    }

    public User? GetByEmail(string email)
    {
        var user = context.Users.SingleOrDefault(u => u.Email == email);
        return user;
    }

    public User Register(User user)
    {
        context.Users.Add(user);
        context.SaveChanges();

        return user;
    }

    public void Update(User updatedUser, int userId)
    {
        var user = context.Users.Find(userId)!;
        user.Name = updatedUser.Name;
        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;
        user.TodoLists = updatedUser.TodoLists;

        context.SaveChanges();
    }

    public void DeleteById(int id)
    {
        var user = context.Users.Find(id)!;
        context.Users.Remove(user);
        context.SaveChanges();
    }
}