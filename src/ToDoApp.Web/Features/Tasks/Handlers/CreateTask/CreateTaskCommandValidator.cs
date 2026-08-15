using FluentValidation;
using ToDoApp.Web.Features.Tasks.Dtos;

namespace ToDoApp.Web.Features.Tasks.Handlers.CreateTask;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        base.RuleFor(e => e.Task).SetValidator(new TaskDtoValidator());
    }
}