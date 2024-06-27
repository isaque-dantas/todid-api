using FluentValidation;
using TodoAPI.Models;

namespace TodoAPI.Requests;

public class UpdateTodoListRequest : TodoListDto
{
    public new class Validator : AbstractValidator<TodoListDto>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotNull().Length(2, 64);
            RuleFor(x => x.Color).NotNull().Length(6);
            RuleForEach(x => x.TodoItems).SetValidator(new TodoItemDto.Validator());
        }
    }

    public class UpdateTodoItemRequest : TodoItemDto
    {
        private new class Validator : AbstractValidator<TodoItemDto>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotNull().Length(2, 64);
                RuleFor(x => x.IsComplete).NotNull();
            }
        }
    }
}