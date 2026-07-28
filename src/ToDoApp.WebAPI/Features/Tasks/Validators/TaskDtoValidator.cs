using FluentValidation;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.WebAPI.Features.Tasks.Dtos;

namespace ToDoApp.WebAPI.Features.Tasks.Validators;

public sealed class TaskDtoValidator : AbstractValidator<TaskDto>
{
    public TaskDtoValidator()
    {
        base.RuleFor(e => e.Title).MaximumLength(TaskConstants.TitleMaxLength).NotEmpty();
    }
}