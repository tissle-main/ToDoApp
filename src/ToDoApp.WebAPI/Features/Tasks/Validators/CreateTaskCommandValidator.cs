using FluentValidation;
using ToDoApp.WebAPI.Features.Tasks.Handlers;

namespace ToDoApp.WebAPI.Features.Tasks.Validators;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        base.RuleFor(e => e.Task).SetValidator(new TaskDtoValidator());
    }
}