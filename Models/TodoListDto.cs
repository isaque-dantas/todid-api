using FluentValidation;

namespace TodoAPI.Models;

public class TodoListDto
{
    public string? Name { get; set; }
    public string? Color { get; set; }
    public List<TodoItemDto>? TodoItems { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    
    public TodoList ToTodoList()
    {
        var todoItems = new List<TodoItem>();
        TodoItems?.ForEach(todoItem => { todoItems.Add(todoItem.ToTodoItem()); });

        return new TodoList
        {
            Name = Name!,
            Color = Color!,
            TodoItems = todoItems,
            UserId = UserId,
            User = User
        };
    }

    public class Validator : AbstractValidator<TodoListDto>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotNull().Length(2, 64);
            RuleFor(x => x.Color).NotNull().Length(6);
            RuleForEach(x => x.TodoItems).SetValidator(new TodoItemDto.Validator());
        }
    }
}

