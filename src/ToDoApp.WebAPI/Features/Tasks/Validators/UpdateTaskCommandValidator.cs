using FluentValidation;
using ToDoApp.WebAPI.Features.Tasks.Handlers;

namespace ToDoApp.WebAPI.Features.Tasks.Validators;

public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        base.RuleFor(e => e.Task).SetValidator(new TaskDtoValidator());
    }
}