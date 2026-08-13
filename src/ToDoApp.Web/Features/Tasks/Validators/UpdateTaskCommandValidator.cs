using FluentValidation;
using ToDoApp.Web.Features.Tasks.Handlers;

namespace ToDoApp.Web.Features.Tasks.Validators;

public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        base.RuleFor(e => e.Task).SetValidator(new TaskDtoValidator());
    }
}