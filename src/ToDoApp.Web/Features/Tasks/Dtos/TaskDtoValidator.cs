using FluentValidation;
using ToDoApp.Data.Features.Tasks;

namespace ToDoApp.Web.Features.Tasks.Dtos;

public sealed class TaskDtoValidator : AbstractValidator<TaskDto>
{
    public TaskDtoValidator()
    {
        base.RuleFor(e => e.Title).NotEmpty().MaximumLength(TaskEntityConstants.TitleMaxLength);
        base.RuleFor(e => e.Description).MaximumLength(TaskEntityConstants.DescriptionMaxLength);
    }
}