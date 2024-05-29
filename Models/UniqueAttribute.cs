namespace TodoAPI.Models;

public class UniqueAttribute<T>
{
    public UniqueAttribute()
    {
    }

    public UniqueAttribute(string name, Func<T, User> serviceSearcherMethod, T value)
    {
        Name = name;
        ServiceSearcherMethod = serviceSearcherMethod;
        Value = value;
    }

    public string Name { get; set; }
    public Func<T, User> ServiceSearcherMethod { get; set; }
    public T Value { get; set; }
}