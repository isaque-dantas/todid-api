namespace TodoAPI.Models;

public class UniqueAttribute<T>
{
    public string Name { get; set; }
    public Func<T, User> ServiceSearcherMethod { get; set; }
    public T Value { get; set; }
}