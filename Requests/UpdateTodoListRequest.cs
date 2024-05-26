namespace TodoAPI.Requests;

using FluentValidation;
using Models;

public class UpdateTodoListRequest : TodoListDto
{
    public new class Validator : AbstractValidator<TodoListDto>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotNull().Length(2, 64);
            RuleFor(x => x.Color).NotNull().Length(6);
            RuleForEach(x => x.TodoItems).SetValidator(new UpdateTodoItemRequest.Validator());
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