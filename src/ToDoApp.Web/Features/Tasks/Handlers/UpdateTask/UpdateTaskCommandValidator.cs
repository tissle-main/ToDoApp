using FluentValidation;
using ToDoApp.Web.Features.Tasks.Dtos;

namespace ToDoApp.Web.Features.Tasks.Handlers.UpdateTask;

public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        base.RuleFor(e => e.Task).SetValidator(new TaskDtoValidator());
    }
}