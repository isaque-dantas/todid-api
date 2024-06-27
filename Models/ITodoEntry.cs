namespace TodoAPI.Models;

public interface ITodoEntry
{
    public int Id { get; set; }
    public string? Name { get; set; }
}