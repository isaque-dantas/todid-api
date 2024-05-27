using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;

namespace TodoAPI.Services;

public class UserService(TodoContext context) : ITodoService
{
    public bool EntryExists(int id)
    {
        throw new NotImplementedException();
    }

    public bool AreEmailAndPasswordValid(string email, string password)
    {
        var user = context.Users.AsNoTracking().SingleOrDefault(u => u.Email == email);

        if (user is null)
            return false;

        return user.Password == password;
    }

// TODO: method that convert ClaimPrincipal to User

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
}