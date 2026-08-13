using FluentValidation;
using ToDoApp.Data.Features.Tasks;
using ToDoApp.Web.Features.Tasks.Dtos;

namespace ToDoApp.Web.Features.Tasks.Validators;

public sealed class TaskDtoValidator : AbstractValidator<TaskDto>
{
    public TaskDtoValidator()
    {
        base.RuleFor(e => e.Title).NotEmpty().MaximumLength(TaskEntityConstants.TitleMaxLength);
        base.RuleFor(e => e.Description).MaximumLength(TaskEntityConstants.DescriptionMaxLength);
    }
}