using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Services;

public class UserService(TodoContext context) : ITodoService
{
    public bool EntryExists(int id)
    {
        throw new NotImplementedException();
    }

    public bool AreEmailAndPasswordValid(string email, string password)
    {
        var user = context.Users.AsNoTracking().Single(u => u.Email == email);
        return context.Users.AsNoTracking().Single(u => u.Email == email).Password == password;
    }
}