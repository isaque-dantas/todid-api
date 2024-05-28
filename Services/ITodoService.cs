namespace TodoAPI.Services;

public interface ITodoService
{
    public bool EntryExists(int id);
    public bool UserHasEntry(int id, int userId);
}