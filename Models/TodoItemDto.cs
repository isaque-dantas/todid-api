using FluentValidation;

namespace TodoAPI.Models;

public class TodoItemDto
{
    public string Name { get; set; }
    public bool IsComplete { get; set; }
    public int TodoListId { get; set; }
    
    public TodoItem ToTodoItem()
    {
        return new TodoItem
        {
            Name=Name,
            IsComplete = IsComplete,
            TodoListId = TodoListId
        };
    }
    
    public class Validator : AbstractValidator<TodoItemDto>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotNull().Length(2, 64);
            RuleFor(x => x.IsComplete).NotNull();
            RuleFor(x => x.TodoListId).NotNull();
        }
    }
}

