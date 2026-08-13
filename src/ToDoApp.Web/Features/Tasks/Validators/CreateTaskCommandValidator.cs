using FluentValidation;
using ToDoApp.Web.Features.Tasks.Handlers;

namespace ToDoApp.Web.Features.Tasks.Validators;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        base.RuleFor(e => e.Task).SetValidator(new TaskDtoValidator());
    }
}